using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    [SerializeField] GameObject roomPrefab;

    RoomScript[,] rooms = null;

    [SerializeField] int numX = 10;
    [SerializeField] int numY = 10;

    float roomWidth;
    float roomHeight;

    Stack<RoomScript> stack = new Stack<RoomScript>();

    bool generating = false;

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
        Camera.main.transform.position = new Vector3
            (numX * (roomWidth - 1) / 2,
            numY * (roomHeight - 1) / 2,
            -100.0f);

        float min_value = Mathf.Min
            (numX * (roomWidth - 1),
            numY * roomHeight - 1);

        Camera.main.orthographicSize = min_value * 0.75f;

    }

    private void Start()
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

                room.name = "Room_" + i.ToString() + "_" + j.ToString();
                rooms[i, j] = room.GetComponent<RoomScript>();
                rooms[i, j].Index = new Vector2Int(i, j);
            }
        }

        SetCamera();
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
        StartCoroutine(Coroutine_Generate());
    }

    IEnumerator Coroutine_Generate()
    {
        generating = true;
        bool flag = false;
        while (!flag)
        {
            flag = GenerateStep();
            yield return new WaitForSeconds(0.05f);
        }
        generating = false;
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

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            if (!generating)
            {
                CreateMaze();
            }
        }
    }
}


