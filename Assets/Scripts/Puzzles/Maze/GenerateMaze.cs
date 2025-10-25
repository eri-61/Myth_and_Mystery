using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;
    [SerializeField] private GameObject loadingPanel; 

    public RoomScript[,] rooms = null;
    MazePlayerController player;

    [SerializeField] private GameObject playerPrefab;
    private GameObject playerInstance;

    [SerializeField] public int numX = 10;
    [SerializeField] public int numY = 10;

    public float roomWidth ;
    public float roomHeight ;

    Stack<RoomScript> stack = new Stack<RoomScript>();

    bool generating = false;

    public int nextScene = 1;
    public int lostScene = 2;

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

        CreateExit();
        SpawnPlayer();
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

        // Ensure the instance has the "Player" tag (warn if tag not defined)
        try
        {
            playerInstance.tag = "Player";
        }
        catch (Exception)
        {
            Debug.LogWarning("[SpawnPlayer] Tag 'Player' is not defined in project Tags. Add it in Tags & Layers or assign tag on the prefab.");
        }

        // Ensure a Rigidbody2D exists so triggers will fire reliably
        if (playerInstance.GetComponent<Rigidbody2D>() == null)
        {
            var rb = playerInstance.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }

        // Fixed scale — visible for testing
        playerInstance.transform.localScale = new Vector3(1f, 1f, 1f);

        // Set z position to be on top
        playerInstance.transform.position = new Vector3(spawnPos.x, spawnPos.y, -1f);

        // Pass reference to maze
        var controller = playerInstance.GetComponent<MazePlayerController>();
        if (controller != null)
        {
            controller.generateMaze = this;
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
        // Top-right cell (exit)
        RoomScript exitRoom = rooms[numX - 1, numY - 1];

        // Create a small visible "Exit" marker
        Vector3 exitPos = exitRoom.transform.position;

        GameObject exit = new GameObject("ExitZone");
        exit.transform.position = exitPos;
        exit.transform.localScale = new Vector3(roomWidth * 0.8f, roomHeight * 0.8f, 1f);

        BoxCollider2D col = exit.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        // Attach the generic ExitZoneTrigger and wire to this GenerateMaze
        var trigger = exit.GetComponent<ExitZoneTrigger>();
        if (trigger == null) trigger = exit.AddComponent<ExitZoneTrigger>();
        trigger.mazeGenSimple = this;

        SpriteRenderer rend = exit.AddComponent<SpriteRenderer>();
        rend.color = new Color(0, 1, 0, 0.3f); // green transparent
        rend.sortingOrder = 10;

        Debug.Log(" Exit created at: " + exitPos);
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

    public void GameWin()
    {
        Debug.Log("🎉 You escaped the maze!");
        if (playerInstance != null)
            playerInstance.GetComponent<MazePlayerController>().enabled = false;

       SceneManager.LoadScene(nextScene);
    }

    public void GameLost()
    {
        SceneManager.LoadScene(lostScene);

    }

}


