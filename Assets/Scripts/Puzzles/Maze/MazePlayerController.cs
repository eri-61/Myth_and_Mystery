using UnityEngine;

public class MazePlayerController : MonoBehaviour
{
    public GenerateMaze generateMaze;
    public Vector2Int currentCell;

    private Camera cam;
    private bool isDragging = false;
    private Vector3 dragStartWorldPos;
    private Vector2Int dragDirection = Vector2Int.zero;

    private void Awake()
    {
        generateMaze = FindObjectOfType<GenerateMaze>();
    }
    private void Start()
    {
        cam = Camera.main;
    }

    private void OnMouseDown()
    {
        isDragging = true;
        dragStartWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        dragStartWorldPos.z = 0;
    }

    private void OnMouseUp()
    {
        if (!isDragging || generateMaze == null)
            return;

        isDragging = false;

        Vector3 dragEndWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        dragEndWorldPos.z = 0;

        Vector3 dragVector = dragEndWorldPos - dragStartWorldPos;

        // Decide direction of movement based on drag
        if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
        {
            dragDirection = dragVector.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else
        {
            dragDirection = dragVector.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        TryMoveToNextRoom();
    }

    private void TryMoveToNextRoom()
    {
        if (generateMaze == null) return;

        RoomScript currentRoom = generateMaze.rooms[currentCell.x, currentCell.y];

        // Convert drag direction to room direction enum
        RoomScript.Direction dir = RoomScript.Direction.NONE;
        if (dragDirection == Vector2Int.up) dir = RoomScript.Direction.TOP;
        else if (dragDirection == Vector2Int.down) dir = RoomScript.Direction.BOTTOM;
        else if (dragDirection == Vector2Int.left) dir = RoomScript.Direction.LEFT;
        else if (dragDirection == Vector2Int.right) dir = RoomScript.Direction.RIGHT;

        // If there’s a wall, stop movement
        if (!currentRoom.CanMove(dir))
        {
            Debug.Log("Wall in that direction!");
            return;
        }

        // Check if trying to move right from the top-right cell (exit)
        if (currentCell.x == generateMaze.numX - 1 && currentCell.y == generateMaze.numY - 1 && dragDirection == Vector2Int.right)
        {
            Debug.Log(" You escaped the maze!");
            // Optional: trigger scene change or animation here
            return;
        }


        // Calculate next cell position
        Vector2Int nextCell = currentCell;
        switch (dir)
        {
            case RoomScript.Direction.TOP: nextCell.y += 1; break;
            case RoomScript.Direction.BOTTOM: nextCell.y -= 1; break;
            case RoomScript.Direction.LEFT: nextCell.x -= 1; break;
            case RoomScript.Direction.RIGHT: nextCell.x += 1; break;
        }

        // Make sure it’s inside the maze
        if (nextCell.x < 0 || nextCell.x >= generateMaze.numX || nextCell.y < 0 || nextCell.y >= generateMaze.numY)
            return;

        // Move to next room
        RoomScript nextRoom = generateMaze.rooms[nextCell.x, nextCell.y];
        transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, -1f);

        currentCell = nextCell;

        Debug.Log("$Moved to room {currentCell}");
    }


}
