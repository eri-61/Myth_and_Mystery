using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float playerSizeFactor = 0.6f;

    [Header("Room")]
    [SerializeField] GameObject roomPrefab;
    [SerializeField] public GameObject exitPrefab;

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
    }

    private void SetCamera()
    {
        if (roomWidth <= 0f || roomHeight <= 0f || numX <= 0 || numY <= 0)
        {
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
            return;
        }

        cam.transform.position = center;

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float padding = 1.10f;

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
            return;
        }

        if (rooms == null)
        {
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

            try
            {
                collectible.tag = "Collectibles";
            }
            catch (Exception)
            {
            }

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
        if (rooms == null || rooms[0, 0] == null) return;
        if (enemyPrefab == null) return;

        RoomScript startRoom = rooms[0, 0];
        Vector3 spawnPos = startRoom.transform.position;

        enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
        enemyInstance.name = "KapreEnemy";

        // Ensure enemy has a trigger collider so OnTriggerEnter2D will fire
        Collider2D enemyCol = enemyInstance.GetComponentInChildren<Collider2D>();
        if (enemyCol == null)
        {
            var bc = enemyInstance.AddComponent<BoxCollider2D>();
            bc.isTrigger = true;
        }
        else
        {
            enemyCol.isTrigger = true;
        }

        // Ensure a Rigidbody2D exists somewhere (player already adds a kinematic Rigidbody2D;
        // if not present on enemy prefab add a kinematic one to be safe)
        Rigidbody2D enemyRb = enemyInstance.GetComponent<Rigidbody2D>();
        if (enemyRb == null)
        {
            enemyRb = enemyInstance.AddComponent<Rigidbody2D>();
            enemyRb.bodyType = RigidbodyType2D.Kinematic;
            enemyRb.simulated = true;
        }

        float desiredWorldSize = Mathf.Min(roomWidth, roomHeight) * enemySizeFactor;
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

                sr.transform.localPosition = Vector3.zero;
                sr.transform.localRotation = Quaternion.identity;

                sr.sortingOrder = Mathf.Max(sr.sortingOrder, 50);

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
        }

        MazeEnemyController enemyCtrl = enemyInstance.GetComponent<MazeEnemyController>();
        if (enemyCtrl != null)
            enemyCtrl.Initialize(this, playerInstance);

        enemySpawned = true;
    }

    public void CollectibleCollected(string name)
    {
        // defensive: calculate total safely
        int total = TotalCollectibles;

        if (total == 0)
        {
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
        SceneManager.LoadScene(gameOver);

    }

    public void OnPlayerExit()
    {
        if (collectedCount >= TotalCollectibles)
        {
            if (text != null)
                text.text = "You escaped the forest!";

            else
            {
                Debug.LogWarning("[GenerateMaze] UI text reference is not assigned; skipping victory UI update.");
            }

            SceneManager.LoadScene(gameWin);
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

        // Spawn Kapre after 4 moves
        if (!enemySpawned && playerMoves >= 4)
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
        Debug.Log($"[DEBUG] CreateExit() called — rooms={rooms}, numX={numX}, numY={numY}");

        if (rooms != null)
        {
            Debug.Log($"[DEBUG] rooms.GetLength(0)={rooms.GetLength(0)}, rooms.GetLength(1)={rooms.GetLength(1)}");
            Debug.Log($"[DEBUG] Trying to access exit room at [{numX - 1}, {numY - 1}]");
            if (numX - 1 < rooms.GetLength(0) && numY - 1 < rooms.GetLength(1))
            {
                var r = rooms[numX - 1, numY - 1];
                Debug.Log($"[DEBUG] rooms[{numX - 1},{numY - 1}] exists? {r != null}");
            }
        }
        else
        {
            Debug.LogWarning("[DEBUG] rooms array is null at CreateExit.");
        }

        if (exitCreated) return;

        if (rooms == null)
        {
            Debug.LogWarning("[CreateExit] rooms array is null. Cannot create exit.");
            return;
        }

        if (numX <= 0 || numY <= 0)
        {
            Debug.LogWarning("[CreateExit] invalid maze dimensions.");
            return;
        }

        int ex = numX - 1;
        int ey = numY - 1;

        if (ex < 0 || ey < 0 || ex >= rooms.GetLength(0) || ey >= rooms.GetLength(1))
        {
            Debug.LogWarning($"[CreateExit] exit coordinates out of range: {ex},{ey}");
            return;
        }

        RoomScript exitRoom = rooms[ex, ey];
        if (exitRoom == null)
        {
            Debug.LogWarning($"[CreateExit] exit room at {ex},{ey} is null.");
            return;
        }

        Vector3 exitPos = exitRoom.transform.position;
        GameObject exitObj = null;

        try
        {
            // If no prefab provided: fallback simple trigger
            if (exitPrefab == null)
            {
                Debug.LogWarning("[CreateExit] exitPrefab is not assigned. Creating fallback ExitZone GameObject.");
                exitObj = new GameObject("ExitZone");
                exitObj.transform.SetParent(transform);
                exitObj.transform.position = new Vector3(exitPos.x, exitPos.y, -0.2f);

                var bc = exitObj.AddComponent<BoxCollider2D>();
                bc.isTrigger = true;
                bc.size = new Vector2(roomWidth * 0.8f, roomHeight * 0.8f);

                var trigger = exitObj.AddComponent<ExitZoneTrigger>();
                trigger.mazeGen = this;

                exitCreated = true;
                Debug.Log("[CreateExit] fallback ExitZone created.");
                return;
            }

            // Instantiate prefab
            exitObj = Instantiate(exitPrefab, exitPos, Quaternion.identity, transform);
            exitObj.name = "ExitZone";
            if (!exitObj.activeInHierarchy) exitObj.SetActive(true);

            // Find colliders (include inactive children)
            Collider2D[] exitColliders = exitObj.GetComponentsInChildren<Collider2D>(true);

            // If no colliders found, add a BoxCollider2D to the root
            if (exitColliders == null || exitColliders.Length == 0)
            {
                Debug.LogWarning("[CreateExit] No Collider2D found on exitPrefab. Adding fallback BoxCollider2D to root.");
                var bc = exitObj.AddComponent<BoxCollider2D>();
                bc.isTrigger = true;
                bc.size = new Vector2(roomWidth * 0.8f, roomHeight * 0.8f);
                exitColliders = new Collider2D[] { bc };
            }

            // Ensure all colliders are triggers and pick a collider owner
            GameObject colliderOwner = null;
            for (int i = 0; i < exitColliders.Length; i++)
            {
                var c = exitColliders[i];
                if (c == null) continue;
                c.isTrigger = true;
                if (colliderOwner == null) colliderOwner = c.gameObject;
            }

            if (colliderOwner == null)
            {
                // last-resort: use root object
                colliderOwner = exitObj;
                var bc = colliderOwner.GetComponent<Collider2D>();
                if (bc == null) bc = colliderOwner.AddComponent<BoxCollider2D>();
                bc.isTrigger = true;
                Debug.LogWarning("[CreateExit] colliderOwner was null; attached BoxCollider2D to root.");
            }

            // Attach or find ExitZoneTrigger on the collider owner and wire the generator
            var exitTrigger = colliderOwner.GetComponent<ExitZoneTrigger>();
            if (exitTrigger == null) exitTrigger = colliderOwner.AddComponent<ExitZoneTrigger>();
            exitTrigger.mazeGen = this;

            Debug.Log($"[CreateExit] exitObj created: {exitObj.name}, colliderOwner: {colliderOwner.name}, colliders: {exitColliders.Length}");

            // Position the exit slightly above floor so it renders correctly
            exitObj.transform.position = new Vector3(exitPos.x, exitPos.y, -0.2f);

            // Immediate overlap detection - if player already standing on exit, trigger exit now
            if (playerInstance != null)
            {
                Vector2 playerPos = playerInstance.transform.position;
                Collider2D[] overlaps = Physics2D.OverlapPointAll(playerPos);

                bool playerInsideExit = false;
                foreach (var oc in overlaps)
                {
                    if (oc == null) continue;
                    // check if the overlap collider belongs to the exit collider owner (or its children)
                    if (oc.gameObject == colliderOwner || oc.transform.IsChildOf(colliderOwner.transform))
                    {
                        playerInsideExit = true;
                        break;
                    }
                }

                if (playerInsideExit)
                {
                    Debug.Log("[CreateExit] Player already overlaps new exit — invoking OnPlayerExit()");
                    OnPlayerExit();
                }
            }

            exitCreated = true;
            Debug.Log($"Exit prefab instantiated at room: {ex},{ey}  pos: {exitPos}");
        }
        catch (Exception x)
        {
            Debug.LogException(x);
        }
    }

}


