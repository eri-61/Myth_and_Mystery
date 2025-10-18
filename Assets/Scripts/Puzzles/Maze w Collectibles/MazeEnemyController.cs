using UnityEngine;
using System.Collections.Generic;

public class MazeEnemyController : MonoBehaviour
{
    private GenerateMaze_wCollectibles maze;
    private GameObject player;
    private Vector2Int currentCell;

    public void Initialize(GenerateMaze_wCollectibles mazeRef, GameObject playerRef)
    {
        maze = mazeRef;
        player = playerRef;
        currentCell = WorldToCell(transform.position);
    }

    private Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / maze.roomWidth);
        int y = Mathf.RoundToInt(worldPos.y / maze.roomHeight);
        return new Vector2Int(Mathf.Clamp(x, 0, maze.numX - 1), Mathf.Clamp(y, 0, maze.numY - 1));
    }

    private Vector3 CellToWorld(Vector2Int cell)
    {
        return maze.rooms[cell.x, cell.y].transform.position;
    }

    public void TakeStep()
    {
        if (player == null) return;

        Vector2Int playerCell = WorldToCell(player.transform.position);
        List<Vector2Int> path = FindPath(currentCell, playerCell);

        if (path.Count > 1)
        {
            currentCell = path[1];
            Vector3 nextPos = CellToWorld(currentCell);
            transform.position = new Vector3(nextPos.x, nextPos.y, -0.5f);
        }
    }

    private List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == goal) break;

            foreach (RoomScript.Direction dir in System.Enum.GetValues(typeof(RoomScript.Direction)))
            {
                if (dir == RoomScript.Direction.NONE) continue;
                if (!maze.rooms[current.x, current.y].CanMove(dir)) continue;

                Vector2Int next = current;
                switch (dir)
                {
                    case RoomScript.Direction.TOP: next.y++; break;
                    case RoomScript.Direction.BOTTOM: next.y--; break;
                    case RoomScript.Direction.LEFT: next.x--; break;
                    case RoomScript.Direction.RIGHT: next.x++; break;
                }

                if (next.x < 0 || next.y < 0 || next.x >= maze.numX || next.y >= maze.numY) continue;

                if (!cameFrom.ContainsKey(next))
                {
                    cameFrom[next] = current;
                    queue.Enqueue(next);
                }
            }
        }

        // Reconstruct path
        List<Vector2Int> path = new List<Vector2Int>();
        if (!cameFrom.ContainsKey(goal)) return path;

        Vector2Int step = goal;
        while (step != start)
        {
            path.Insert(0, step);
            step = cameFrom[step];
        }
        path.Insert(0, start);

        return path;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enemy caught the player!");
            maze.GameOver();
        }
    }
}
