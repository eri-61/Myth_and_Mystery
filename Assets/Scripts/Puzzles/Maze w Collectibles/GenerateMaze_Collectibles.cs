using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GenerateMaze_wCollectibles : MonoBehaviour
{
    #region Variables
    [SerializeField] GameObject roomPrefab;
    [SerializeField] private GameObject loadingPanel; 

    public RoomScript[,] rooms = null;

    [Header("Player")]
    MazePlayerController_wCollectibles player;
    [SerializeField] private GameObject playerPrefab;
    private GameObject playerInstance;

    [Header("Room")]
    [SerializeField] public int numX = 10;
    [SerializeField] public int numY = 10;
    private bool exitCreated = false;

    public float roomWidth ;
    public float roomHeight ;

    public GameObject exitPrefab;

    [Header("Collectibles & Enemy")]
    [SerializeField] private GameObject[] collectiblePrefabs;
    private List<GameObject> spawnedCollectibles = new List<GameObject>();
    private int collectedCount = 0;
    private int totalCollectibles => collectiblePrefabs.Length;


    [SerializeField] private GameObject enemyPrefab;
    private GameObject enemyInstance; 
    public int playerMoves = 0;
    public bool enemySpawned = false;

    [Header("Scene")]
    public int gameOver = 1;
    public int gameWin = 1;

    Stack<RoomScript> stack = new Stack<RoomScript>();

    bool generating = false;
    #endregion

    private void GetRoomSize()
    {
        SpriteRenderer[] spriteRenderers = roomPrefab.GetComponentsInChildren<SpriteRenderer>();

        Vector3 minBounds = Vector3.positiveInfinity;
        Vector3 maxBounds = Vector3.negativeInfinity;

        foreach (SpriteRenderer ren in spriteRenderers)
        {
            minBounds = Vector3.Min(minBounds, ren.bounds.min);
            maxBounds = Vector3.Max(maxBounds, ren.bounds.max);
        }

        roomWidth = maxBounds.x - minBounds.x;
        roomHeight = maxBounds.y - minBounds.y;
    }

    private void SetCamera()
    {
        // Calculate the maze's total width and height in world units
        float mazeWidth = numX * roomWidth;
        float mazeHeight = numY * roomHeight;

        // Center the camera on the maze
        Camera.main.transform.position = new Vector3(
            mazeWidth / 2 - roomWidth / 2,
            mazeHeight / 2 - roomHeight / 2,
            -10f 
        );

        // Adjust zoom so the whole maze fits, but not too small
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = mazeWidth / mazeHeight;

        float orthographicSize;

        if (screenAspect >= targetAspect)
        {
            // Screen is wider than maze
            orthographicSize = mazeHeight / 2f;
        }
        else
        {
            // Screen is taller than maze
            orthographicSize = (mazeWidth / 2f) / screenAspect;
        }

        // Apply a zoom-in factor (smaller number = more zoom)
        Camera.main.orthographicSize = orthographicSize * 1f;
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
        SpawnCollectibles();
    }

    private void SpawnCollectibles()
    {
        if (collectiblePrefabs == null || collectiblePrefabs.Length == 0)
        {
            Debug.LogWarning("⚠️ No collectible prefabs assigned!");
            return;
        }

        HashSet<Vector2Int> usedRooms = new HashSet<Vector2Int>();
        usedRooms.Add(new Vector2Int(0, 0));
        usedRooms.Add(new Vector2Int(numX - 1, numY - 1)); 

        foreach (GameObject prefab in collectiblePrefabs)
        {
            // Find a random unused room
            Vector2Int pos;
            do
            {
                pos = new Vector2Int(UnityEngine.Random.Range(0, numX), UnityEngine.Random.Range(0, numY));
            }
            while (usedRooms.Contains(pos));

            usedRooms.Add(pos);

            // Spawn collectible
            RoomScript room = rooms[pos.x, pos.y];
            Vector3 spawnPos = room.transform.position;
            GameObject collectible = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

            collectible.name = $"Collectible_{prefab.name}";
            spawnedCollectibles.Add(collectible);
        }

        Debug.Log($"✅ Spawned {spawnedCollectibles.Count} collectibles across the maze.");
    }


    public void SpawnEnemy()
    {
        if (enemySpawned) return;

        RoomScript startRoom = rooms[0, 0];
        Vector3 spawnPos = startRoom.transform.position;

        enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemyInstance.name = "KapreEnemy";

        MazeEnemyController enemyCtrl = enemyInstance.GetComponent<MazeEnemyController>();
        enemyCtrl.Initialize(this, playerInstance);

        Debug.Log("Kapre spawned at (0,0)");
    }



    public void CollectibleCollected(string name)
    {
        collectedCount++;
        Debug.Log($"Collected {name} ({collectedCount}/{totalCollectibles})");

        if (collectedCount >= totalCollectibles)
        {
            Debug.Log("🎉 All collectibles gathered! Exit is now open!");
            CreateExit();
        }
    }

    private void RemoveWall(int x, int y, RoomScript.Direction dir)
    {
        if (dir != RoomScript.Direction.NONE)
        {
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

        if (opp != RoomScript.Direction.NONE)
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
                        if (!rooms[x, y].visited)
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
                        if (!rooms[x, y].visited)
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
                        if (!rooms[x, y].visited)
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
                        if (!rooms[x, y].visited)
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

        stack.Push(rooms[0, 0]);
    }

    private void SpawnPlayer()
    {
        if (rooms == null || rooms[0, 0] == null)
        {
            Debug.LogError("Rooms not ready, cannot spawn player.");
            return;
        }

        // Lower-left room (0,0)
        RoomScript startRoom = rooms[0, 0];
        Vector3 spawnPos = startRoom.transform.position;

        // Spawn player
        playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerInstance.name = "Player";
        playerInstance.transform.SetParent(transform); // Keep hierarchy tidy

        // Fixed scale — visible for testing
        playerInstance.transform.localScale = new Vector3(100f, 100f, 1f);

        // Set z position to be on top
        playerInstance.transform.position = new Vector3(spawnPos.x, spawnPos.y, -1f);

        // Pass reference to maze
        var controller = playerInstance.GetComponent<MazePlayerController_wCollectibles>();
        if (controller != null)
        {
            controller.CE = this;
            controller.currentCell = new Vector2Int(0, 0);
        }

        Debug.Log($"✅ Player spawned at {spawnPos} with scale {playerInstance.transform.localScale}");
    }

    private void Reset()
    {
        for (int i = 0; i < numX; ++i)
        {
            for (int j = 0; j < numY; ++j)
            {
                rooms[i, j].SetDirFlag(RoomScript.Direction.TOP, true);
                rooms[i, j].SetDirFlag(RoomScript.Direction.RIGHT, true);
                rooms[i, j].SetDirFlag(RoomScript.Direction.BOTTOM, true);
                rooms[i, j].SetDirFlag(RoomScript.Direction.LEFT, true);
                rooms[i, j].visited = false;
            }
        }
    }

    private void CreateExit()
    {
        if (exitCreated)
        {
            return;
        }
        else
        {
            // Top-right cell (exit)
            RoomScript exitRoom = rooms[numX - 1, numY - 1];

            // Open right wall
            exitRoom.SetDirFlag(RoomScript.Direction.RIGHT, false);

            // Create a small visible "Exit" marker outside the maze
            Vector3 exitPos = exitRoom.transform.position + new Vector3(roomWidth, 0, 0);

            GameObject exit = new GameObject("ExitZone");
            exit.transform.position = exitPos;
            exit.transform.localScale = new Vector3(roomWidth * 0.8f, roomHeight * 0.8f, 1f);

            BoxCollider2D col = exit.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            SpriteRenderer rend = exit.AddComponent<SpriteRenderer>();
            rend.color = new Color(0, 1, 0, 0.3f); // green transparent
            rend.sortingOrder = 10;

            exit.AddComponent<ExitZoneTrigger>();

            Debug.Log(" Exit created at: " + exitPos);
            exitCreated = true;
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
                rooms[i, j] = roomScript;
                rooms[i, j].Index = new Vector2Int(i, j);
            }
        }

        SetCamera();
    }
    public void GameOver()
    {
        Debug.Log("💀 Game Over! The enemy caught you!");

        SceneController.Instance.LoadScene(gameOver);

    }

    public void OnPlayerExit()
    {
        if (collectedCount >= totalCollectibles)
        {
            Debug.Log("🎉 Player exited the maze — You Win!");
            SceneController.Instance.LoadScene(gameWin);
        }
        else
        {
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
            enemyInstance.GetComponent<MazeEnemyController>().TakeStep();
        }
    }
}


