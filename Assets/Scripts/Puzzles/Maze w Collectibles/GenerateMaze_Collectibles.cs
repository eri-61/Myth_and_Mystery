using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GenerateMaze_wCollectibles : MonoBehaviour
{
    #region Variables
    [Header("UI Objects")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI text;
    public RoomScript[,] rooms = null;

    [Header("Player")]
    MazePlayerController_wCollectibles player;
    [SerializeField] private GameObject playerPrefab;
    private GameObject playerInstance;
    [SerializeField] private float playerSizeFactor = 0.6f; // adjustable in Inspector

    [Header("Room")]
    [SerializeField] GameObject roomPrefab;
    public GameObject exitInstance;


    [SerializeField] private Sprite exitSprite;

    [SerializeField] public int numX = 10;
    [SerializeField] public int numY = 10;
    private bool exitCreated = false;

    public float roomWidth;
    public float roomHeight;

    Stack<RoomScript> stack = new Stack<RoomScript>();

    [Header("Collectible")]
    [SerializeField] private GameObject[] collectiblePrefabs;
    private List<GameObject> spawnedCollectibles = new List<GameObject>();
    private int collectedCount = 0;
    private int TotalCollectibles => (collectiblePrefabs != null && collectiblePrefabs.Length > 0) ? collectiblePrefabs.Length : spawnedCollectibles.Count;

    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;
    private GameObject enemyInstance;
    [SerializeField] private float enemySizeFactor = 0.6f; // adjustable in Inspector
    public int playerMoves = 0;
    public bool enemySpawned = false;

    [Header("Scene")]
    public int gameOver = 1;
    public int gameWin = 1;

    [Header("Maze")]
    bool generating = false;

    [Header("Maze Options")]
    public bool allowExtraConnections = true;
    public int extraConnections = 6;
    public int minOpenDirectionsForCollectible = 2;
    #endregion

    private void GetRoomSize()
    {
        if (roomPrefab == null)
        {
            Debug.LogWarning("[GenerateMaze] GetRoomSize: roomPrefab is not assigned. Using default size 1x1.");
            roomWidth = 1f;
            roomHeight = 1f;
            return;
        }

        SpriteRenderer[] spriteRenderers = roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        if (spriteRenderers == null || spriteRenderers.Length == 0)
        {
            Debug.LogWarning("[GenerateMaze] GetRoomSize: roomPrefab has no SpriteRenderer children. Using default size 1x1. Check prefab PPU/graphics.");
            roomWidth = 1f;
            roomHeight = 1f;
            return;
        }

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        foreach (SpriteRenderer ren in spriteRenderers)
        {
            if (ren == null || ren.sprite == null) continue;
            minBounds = Vector3.Min(minBounds, ren.bounds.min);
            maxBounds = Vector3.Max(maxBounds, ren.bounds.max);
        }

        if (minBounds == Vector3.positiveInfinity || maxBounds == Vector3.negativeInfinity)
        {
            Debug.LogWarning("[GenerateMaze] GetRoomSize: couldn't calculate bounds from SpriteRenderers. Using default size 1x1.");
            roomWidth = 1f;
            roomHeight = 1f;
            return;
        }

        roomWidth = maxBounds.x - minBounds.x;
        roomHeight = maxBounds.y - minBounds.y;

        // Safety: avoid zero sizes
        if (roomWidth <= 0f) roomWidth = 1f;
        if (roomHeight <= 0f) roomHeight = 1f;

        Debug.Log($"[GenerateMaze] roomSize calculated: roomWidth={roomWidth:F3}, roomHeight={roomHeight:F3}");
    }

    private void SetCamera()
    {
        if (roomWidth <= 0f || roomHeight <= 0f || numX <= 0 || numY <= 0)
        {
            Debug.LogWarning("[GenerateMaze] SetCamera: invalid room or maze dimensions.");
            return;
        }

        float mazeWidth = numX * roomWidth;
        float mazeHeight = numY * roomHeight;

        Vector3 center = new Vector3(
            mazeWidth / 2f - roomWidth / 2f,
            mazeHeight / 2f - roomHeight / 2f,
            -10f
        );

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("[GenerateMaze] SetCamera: Camera.main is null.");
            return;
        }

        cam.transform.position = center;

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float padding = 1.10f; // slightly larger padding for phone screens

        float targetOrtho;
        float mazeAspect = mazeWidth / mazeHeight;

        if (screenAspect >= mazeAspect)
        {
            targetOrtho = (mazeHeight / 2f) * padding;
        }
        else
        {
            targetOrtho = ((mazeWidth / 2f) / screenAspect) * padding;
        }

        targetOrtho = Mathf.Clamp(targetOrtho, 1f, 2000f);

        cam.orthographic = true;
        cam.orthographicSize = targetOrtho;

        Debug.Log($"[GenerateMaze] SetCamera: maze {mazeWidth:F2}x{mazeHeight:F2}, screenAspect={screenAspect:F2}, targetOrtho={targetOrtho:F2}");

        // Verify if the maze fits; if not, scale maze root to fit viewport
        float viewHeight = cam.orthographicSize * 2f;
        float viewWidth = viewHeight * screenAspect;

        float eps = 0.01f;
        if (mazeWidth > viewWidth + eps || mazeHeight > viewHeight + eps)
        {
            float scaleX = viewWidth / mazeWidth;
            float scaleY = viewHeight / mazeHeight;
            float scale = Mathf.Min(scaleX, scaleY);

            scale = Mathf.Min(1f, scale);
            scale = Mathf.Max(scale, 0.01f);

            transform.localScale = new Vector3(scale, scale, 1f);

            cam.transform.position = new Vector3(
                (mazeWidth * scale) / 2f - (roomWidth * scale) / 2f,
                (mazeHeight * scale) / 2f - (roomHeight * scale) / 2f,
                -10f
            );

            Debug.Log($"[GenerateMaze] Maze scaled by {scale:F3} to fit viewport (view {viewWidth:F2}x{viewHeight:F2}).");
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    private void Start()
    {
        StartCoroutine(GenerateMazeWithLoading());
    }

    private IEnumerator GenerateMazeWithLoading()
    {
        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        InitializeRooms();
        CreateMaze();

        while (generating)
            yield return null;

        yield return StartCoroutine(GenerateMazeInstant());

        foreach (Transform child in transform)
            child.gameObject.SetActive(true);

        if (loadingPanel != null)
            loadingPanel.SetActive(false);
    }

    IEnumerator GenerateMazeInstant()
    {
        generating = true;
        bool done = false;
        while (!done)
        {
            done = GenerateStep();
            yield return null;
        }

        generating = false;
        yield return null;

        SpawnPlayer();

        if (allowExtraConnections)
            CreateExtraConnections(extraConnections);

        SpawnCollectibles();
    }

    private void SpawnCollectibles()
    {
        if (collectiblePrefabs == null || collectiblePrefabs.Length == 0)
        {
            Debug.LogWarning("⚠️ No collectible prefabs assigned!");
            return;
        }

        if (rooms == null)
        {
            Debug.LogWarning("[GenerateMaze] SpawnCollectibles: rooms is null.");
            return;
        }

        HashSet<Vector2Int> usedRooms = new HashSet<Vector2Int>();
        usedRooms.Add(new Vector2Int(0, 0)); // start
        usedRooms.Add(new Vector2Int(numX - 1, numY - 1)); // exit

        List<Vector2Int> candidates = new List<Vector2Int>();
        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                var pos = new Vector2Int(x, y);
                if (usedRooms.Contains(pos)) continue;

                RoomScript r = rooms[x, y];
                if (r == null) continue;

                int openCount = CountOpenDirections(r);
                if (openCount >= minOpenDirectionsForCollectible)
                    candidates.Add(pos);
            }
        }

        if (candidates.Count < collectiblePrefabs.Length)
        {
            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (usedRooms.Contains(pos) || candidates.Contains(pos)) continue;
                    RoomScript r = rooms[x, y];
                    if (r == null) continue;
                    int openCount = CountOpenDirections(r);
                    if (openCount >= 1)
                        candidates.Add(pos);
                }
            }
        }

        if (candidates.Count < collectiblePrefabs.Length)
        {
            for (int x = 0; x < numX; x++)
            {
                for (int y = 0; y < numY; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (usedRooms.Contains(pos) || candidates.Contains(pos)) continue;
                    candidates.Add(pos);
                }
            }
        }

        System.Random rng = new System.Random();
        candidates = candidates.OrderBy(_ => rng.Next()).ToList();

        int spawnIndex = 0;
        foreach (GameObject prefab in collectiblePrefabs)
        {
            if (spawnIndex >= candidates.Count)
            {
                Debug.LogWarning("[GenerateMaze] Not enough candidate rooms for all collectibles.");
                break;
            }

            Vector2Int pos = candidates[spawnIndex++];
            usedRooms.Add(pos);

            RoomScript room = rooms[pos.x, pos.y];
            if (room == null) continue;
            Vector3 spawnPos = room.transform.position;
            GameObject collectible = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

            collectible.name = $"Collectible_{prefab.name}";
            spawnedCollectibles.Add(collectible);

            // Ensure tag for pickup detection (safe)
            try
            {
                collectible.tag = "Collectibles";
            }
            catch (Exception)
            {
                Debug.LogWarning("[GenerateMaze] Tag 'Collectibles' not defined. Set the tag in the Editor for better identification.");
            }

            // Ensure it has a trigger collider so OverlapCircle / trigger checks find it
            Collider2D existingCol = collectible.GetComponent<Collider2D>();
            if (existingCol == null)
            {
                var col = collectible.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
            }
            else
            {
                existingCol.isTrigger = true;
            }

            // Optional: add kinematic rigidbody so physics triggers behave consistently
            if (collectible.GetComponent<Rigidbody2D>() == null)
            {
                var rb = collectible.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.simulated = true;
            }
        }

        Debug.Log($"Spawned {spawnedCollectibles.Count} collectibles across the maze.");
    }

    private int CountOpenDirections(RoomScript room)
    {
        int open = 0;
        if (room == null) return open;
        foreach (RoomScript.Direction dir in Enum.GetValues(typeof(RoomScript.Direction)))
        {
            if (dir == RoomScript.Direction.NONE) continue;
            try
            {
                if (room.CanMove(dir)) open++;
            }
            catch (Exception)
            {
            }
        }
        return open;
    }


    private void CreateExtraConnections(int attempts)
    {
        if (rooms == null) return;

        int created = 0;
        int tries = 0;
        System.Random rnd = new System.Random();

        while (created < attempts && tries < attempts * 10)
        {
            tries++;
            int x = rnd.Next(0, numX);
            int y = rnd.Next(0, numY);

            RoomScript.Direction dir = (RoomScript.Direction)Enum.GetValues(typeof(RoomScript.Direction))
                .Cast<RoomScript.Direction>()
                .Where(d => d != RoomScript.Direction.NONE)
                .OrderBy(_ => rnd.Next())
                .First();

            if ((x == 0 && y == 0) || (x == numX - 1 && y == numY - 1)) continue;

            int nx = x, ny = y;
            switch (dir)
            {
                case RoomScript.Direction.TOP: ny++; break;
                case RoomScript.Direction.BOTTOM: ny--; break;
                case RoomScript.Direction.RIGHT: nx++; break;
                case RoomScript.Direction.LEFT: nx--; break;
                default: continue;
            }

            if (nx < 0 || nx >= numX || ny < 0 || ny >= numY) continue;

            RoomScript a = rooms[x, y];
            RoomScript b = rooms[nx, ny];

            if (a == null || b == null) continue;

            if (a.CanMove(dir)) continue;

            RemoveWall(x, y, dir);
            created++;
        }

        if (created > 0)
            Debug.Log($" Created {created} extra connection(s) to introduce loops in the maze.");
    }

    public void SpawnEnemy()
    {
        if (enemySpawned) return;
        if (rooms == null || rooms[0, 0] == null)
        {
            Debug.LogError("[SpawnEnemy] Rooms not initialized.");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogWarning("[SpawnEnemy] enemyPrefab is not assigned.");
            return;
        }

        RoomScript startRoom = rooms[0, 0];
        Vector3 spawnPos = startRoom.transform.position;

        // Instantiate as child so it inherits maze root scaling
        enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
        enemyInstance.name = "KapreEnemy";

        // Desired world size (adjust factor)
        float desiredWorldSize = Mathf.Min(roomWidth, roomHeight) * enemySizeFactor;

        // Account for maze root lossly scale (if maze was scaled to fit viewport)
        float mazeScale = Mathf.Abs(transform.lossyScale.x);
        if (Mathf.Approximately(mazeScale, 0f)) mazeScale = 1f;

        var sr = enemyInstance.GetComponentInChildren<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            Vector2 spriteSize = sr.sprite.bounds.size;
            if (spriteSize.x > 0f && spriteSize.y > 0f)
            {
                float sx = desiredWorldSize / (spriteSize.x * mazeScale);
                float sy = desiredWorldSize / (spriteSize.y * mazeScale);
                float uniform = Mathf.Clamp(Mathf.Min(sx, sy), 0.01f, 100f);
                enemyInstance.transform.localScale = new Vector3(uniform, uniform, 1f);

                // center sprite
                sr.transform.localPosition = Vector3.zero;
                sr.transform.localRotation = Quaternion.identity;

                // ensure visible above tiles
                sr.sortingOrder = Mathf.Max(sr.sortingOrder, 50);
                // set z slightly above room
                enemyInstance.transform.position = new Vector3(spawnPos.x, spawnPos.y, -0.5f);
            }
            else
            {
                enemyInstance.transform.localScale = Vector3.one;
            }
        }
        else
        {
            enemyInstance.transform.localScale = Vector3.one;
            Debug.LogWarning("[SpawnEnemy] Enemy prefab has no SpriteRenderer child; adjust prefab PPU/scale in Editor.");
        }

        MazeEnemyController enemyCtrl = enemyInstance.GetComponent<MazeEnemyController>();
        if (enemyCtrl != null)
            enemyCtrl.Initialize(this, playerInstance);

        enemySpawned = true;
        Debug.Log($"Kapre spawned at {spawnPos} with localScale {enemyInstance.transform.localScale}");
    }

    public void CollectibleCollected(string name)
    {
        // defensive: calculate total safely
        int total = TotalCollectibles;

        if (total == 0)
        {
            Debug.LogWarning("[GenerateMaze] Total collectibles is 0 — cannot progress to exit. Check collectiblePrefabs or spawnedCollectibles.");
            return;
        }

        collectedCount++;
        Debug.Log($"Collected {name} ({collectedCount}/{total})");

        if (collectedCount >= total)
        {
            if (text != null)
                text.text = "All of the friends have been gathered! Exit is now open!";
            else
                Debug.LogWarning("[GenerateMaze] UI text reference is not assigned; skipping UI update.");

            Debug.Log(" All collectibles gathered! Exit is now open!");
            CreateExit();
        }
    }

    private void RemoveWall(int x, int y, RoomScript.Direction dir)
    {
        if (dir != RoomScript.Direction.NONE)
        {
            if (rooms != null && rooms[x, y] != null)
                rooms[x, y].SetDirFlag(dir, false);
        }

        RoomScript.Direction opp = RoomScript.Direction.NONE;

        switch (dir)
        {
            case RoomScript.Direction.TOP:
                if (y < numY - 1)
                {
                    opp = RoomScript.Direction.BOTTOM;
                    ++y;
                }
                break;

            case RoomScript.Direction.BOTTOM:
                if (y > 0)
                {
                    opp = RoomScript.Direction.TOP;
                    --y;
                }
                break;

            case RoomScript.Direction.RIGHT:
                if (x < numX - 1)
                {
                    opp = RoomScript.Direction.LEFT;
                    ++x;
                }
                break;

            case RoomScript.Direction.LEFT:
                if (x > 0)
                {
                    opp = RoomScript.Direction.RIGHT;
                    --x;
                }
                break;
        }

        if (opp != RoomScript.Direction.NONE && rooms != null && rooms[x, y] != null)
        {
            rooms[x, y].SetDirFlag(opp, false);
        }
    }

    public List<Tuple<RoomScript.Direction, RoomScript>> GetNeighboursNotVisited(
    int cx, int cy)
    {
        List<Tuple<RoomScript.Direction, RoomScript>> neighbours = new List<Tuple<RoomScript.Direction, RoomScript>>();
        foreach (RoomScript.Direction dir in Enum.GetValues(typeof(RoomScript.Direction)))
        {
            int x = cx;
            int y = cy;
            switch (dir)
            {
                case RoomScript.Direction.TOP:
                    if (y < numY - 1)
                    {
                        ++y;
                        if (rooms[x, y] != null && !rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<RoomScript.Direction, RoomScript>(
                            RoomScript.Direction.TOP,
                            rooms[x, y]));
                        }
                    }
                    break;

                case RoomScript.Direction.RIGHT:
                    if (x < numX - 1)
                    {
                        ++x;
                        if (rooms[x, y] != null && !rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<RoomScript.Direction, RoomScript>(
                              RoomScript.Direction.RIGHT,
                              rooms[x, y]));
                        }
                    }
                    break;

                case RoomScript.Direction.BOTTOM:
                    if (y > 0)
                    {
                        --y;
                        if (rooms[x, y] != null && !rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<RoomScript.Direction, RoomScript>(
                              RoomScript.Direction.BOTTOM,
                              rooms[x, y]));
                        }
                    }
                    break;

                case RoomScript.Direction.LEFT:
                    if (x > 0)
                    {
                        --x;
                        if (rooms[x, y] != null && !rooms[x, y].visited)
                        {
                            neighbours.Add(new Tuple<RoomScript.Direction, RoomScript>(
                              RoomScript.Direction.LEFT,
                              rooms[x, y]));
                        }
                    }
                    break;

            }
        }
        return neighbours;

    }

    private bool GenerateStep()
    {
        if (stack.Count == 0) return true;
        RoomScript r = stack.Peek();
        var neighbours = GetNeighboursNotVisited(r.Index.x, r.Index.y);
        if (neighbours.Count != 0)
        {
            var index = 0;
            if (neighbours.Count > 1)
            {
                index = UnityEngine.Random.Range(0, neighbours.Count);
            }
            var item = neighbours[index];
            RoomScript neighbour = item.Item2;
            neighbour.visited = true;
            RemoveWall(r.Index.x, r.Index.y, item.Item1);
            stack.Push(neighbour);
        }
        else
        {
            stack.Pop();
        }
        return false;
    }

    public void CreateMaze()
    {
        if (generating) return;
        Reset();

        RemoveWall(0, 0, RoomScript.Direction.BOTTOM);
        RemoveWall(numX - 1, numY - 1, RoomScript.Direction.RIGHT);

        if (rooms != null && rooms[0, 0] != null)
            stack.Push(rooms[0, 0]);
        else
            Debug.LogError("[CreateMaze] Cannot start maze generation, start room missing.");
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[SpawnPlayer] playerPrefab is not assigned.");
            return;
        }

        if (rooms == null || rooms[0, 0] == null)
        {
            Debug.LogError("Rooms not ready, cannot spawn player.");
            return;
        }
        RoomScript startRoom = rooms[0, 0];
        Vector3 spawnPos = startRoom.transform.position;

        playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity, transform);
        playerInstance.name = "Player";

        // Desired world size and maze scale
        float desiredWorldSize = Mathf.Min(roomWidth, roomHeight) * playerSizeFactor;
        float mazeScale = Mathf.Abs(transform.lossyScale.x);
        if (Mathf.Approximately(mazeScale, 0f)) mazeScale = 1f;

        var sr = playerInstance.GetComponentInChildren<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            Vector2 spriteSize = sr.sprite.bounds.size;
            if (spriteSize.x > 0f && spriteSize.y > 0f)
            {
                float sx = desiredWorldSize / (spriteSize.x * mazeScale);
                float sy = desiredWorldSize / (spriteSize.y * mazeScale);
                float uniform = Mathf.Clamp(Mathf.Min(sx, sy), 0.01f, 100f);
                playerInstance.transform.localScale = new Vector3(uniform, uniform, 1f);

                sr.transform.localPosition = Vector3.zero;
                sr.transform.localRotation = Quaternion.identity;
                sr.sortingOrder = Mathf.Max(sr.sortingOrder, 60);
            }
            else
            {
                playerInstance.transform.localScale = Vector3.one;
            }
        }
        else
        {
            playerInstance.transform.localScale = Vector3.one;
            Debug.LogWarning("[SpawnPlayer] Player prefab has no SpriteRenderer child; adjust prefab PPU/scale in Editor.");
        }

        playerInstance.transform.position = new Vector3(spawnPos.x, spawnPos.y, -1f);

        var controller = playerInstance.GetComponent<MazePlayerController_wCollectibles>();
        if (controller != null)
        {
            controller.CE = this;
            controller.currentCell = new Vector2Int(0, 0);
        }

        Debug.Log($"Player spawned at {spawnPos} with scale {playerInstance.transform.localScale}");
    }

    private void Reset()
    {
        if (rooms == null) return;

        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                if (rooms[i, j] == null) continue;
                rooms[i, j].SetDirFlag(RoomScript.Direction.TOP, true);
                rooms[i, j].SetDirFlag(RoomScript.Direction.RIGHT, true);
                rooms[i, j].SetDirFlag(RoomScript.Direction.BOTTOM, true);
                rooms[i, j].SetDirFlag(RoomScript.Direction.LEFT, true);
                rooms[i, j].visited = false;
            }
        }
    }

    private void InitializeRooms()
    {
        GetRoomSize();

        rooms = new RoomScript[numX, numY];

        for (int i = 0; i < numX; i++)
        {
            for (int j = 0; j < numY; j++)
            {
                GameObject room = Instantiate(roomPrefab,
                    new Vector3(i * roomWidth, j * roomHeight, 0.0f),
                    Quaternion.identity);

                room.name = $"Room_{i}_{j}";
                room.transform.SetParent(transform); // keeps things tidy

                var roomScript = room.GetComponent<RoomScript>();
                if (roomScript == null)
                {
                    // Try to add the component to avoid nulls; this may still require
                    // assigning wall fields in the prefab or setting them up in Editor.
                    roomScript = room.AddComponent<RoomScript>();
                    Debug.LogWarning($"[InitializeRooms] Room prefab was missing RoomScript; added at runtime for Room_{i}_{j}. Check prefab.");
                }

                rooms[i, j] = roomScript;
                if (rooms[i, j] != null)
                    rooms[i, j].Index = new Vector2Int(i, j);
            }
        }

        SetCamera();
    }

    public void GameOver()
    {
        Debug.Log("💀 Game Over! The enemy caught you!");

        if (SceneController.Instance != null)
            SceneController.Instance.LoadScene(gameOver);
        else
            Debug.LogError("[GenerateMaze] SceneController.Instance is null. Cannot load GameOver scene.");
    }

    public void OnPlayerExit()
    {
        if (collectedCount >= TotalCollectibles)
        {
            if (text != null)
                text.text = "You escaped the maze!";
            else
                Debug.LogWarning("[GenerateMaze] UI text reference is not assigned; skipping victory UI update.");

            Debug.Log(" Player exited the maze — You Win!");
            if (SceneController.Instance != null)
                SceneController.Instance.LoadScene(gameWin);
            else
                Debug.LogError("[GenerateMaze] SceneController.Instance is null. Cannot load Win scene.");
        }
        else
        {
            if (text != null) text.text = "You haven't collected ALL of the scattered people. Collect them all before escaping.";
            Debug.Log("🚪 You found the exit, but you haven’t collected everything yet!");
        }
    }

    public void OnPlayerMove()
    {
        playerMoves++;

        // Spawn Kapre after 3 moves
        if (!enemySpawned && playerMoves >= 3)
        {
            SpawnEnemy();
            enemySpawned = true;
            return; // Kapre waits for next move before chasing
        }

        // Once spawned, move one step after each player move
        if (enemySpawned && enemyInstance != null)
        {
            var enemyCtrl = enemyInstance.GetComponent<MazeEnemyController>();
            if (enemyCtrl != null)
                enemyCtrl.TakeStep();
        }
    }

    private void CreateExit()
    {
        // Top-right cell (exit)
        RoomScript exitRoom = rooms[numX - 1, numY - 1];

        // Open right wall (optional if you still want a visual passage)
        exitRoom.SetDirFlag(RoomScript.Direction.RIGHT, false);

        // Position the exit directly in the last room
        Vector3 exitPos = exitRoom.transform.position;

        GameObject exit = new GameObject("ExitZone");
        exit.transform.position = exitPos;
        exit.transform.localScale = new Vector3(roomWidth * 0.8f, roomHeight * 0.8f, 1f);

        // Collider setup
        BoxCollider2D col = exit.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        // Sprite setup
        SpriteRenderer rend = exit.AddComponent<SpriteRenderer>();
        if (exitSprite != null)
        {
            rend.sprite = exitSprite;
            rend.color = Color.white; // normal tint
        }
        else
        {
            rend.color = new Color(0, 1, 0, 0.3f); // fallback if no sprite assigned
        }

        rend.sortingOrder = 10;

        Debug.Log("Exit created at room: " + (numX - 1) + "," + (numY - 1) + "  pos: " + exitPos);
    }


}
