using UnityEngine;

public class MazePlayerController_wCollectibles : MonoBehaviour
{
    public GenerateMaze_wCollectibles CE;
    public Vector2Int currentCell;

    private Camera cam;
    private bool isDragging = false;
    private Vector3 dragStartWorldPos;
    private Vector2Int dragDirection = Vector2Int.zero;
    private int moveCount = 0;

    private void Awake()
    {
        CE = FindAnyObjectByType<GenerateMaze_wCollectibles>();
    }

    private void Start()
    {
        cam = Camera.main;

        // Ensure collider for OnMouseDown to work
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
    }

    private void OnMouseDown()
    {
        isDragging = true;
        dragStartWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        dragStartWorldPos.z = 0;
    }

    private void OnMouseUp()
    {
        if (!isDragging || CE == null)
            return;

        isDragging = false;

        Vector3 dragEndWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        dragEndWorldPos.z = 0;

        Vector3 dragVector = dragEndWorldPos - dragStartWorldPos;

        if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
            dragDirection = dragVector.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            dragDirection = dragVector.y > 0 ? Vector2Int.up : Vector2Int.down;

        TryMoveToNextRoom();
    }

    private void TryMoveToNextRoom()
    {
        RoomScript currentRoom = CE.rooms[currentCell.x, currentCell.y];

        RoomScript.Direction dir = RoomScript.Direction.NONE;
        if (dragDirection == Vector2Int.up) dir = RoomScript.Direction.TOP;
        else if (dragDirection == Vector2Int.down) dir = RoomScript.Direction.BOTTOM;
        else if (dragDirection == Vector2Int.left) dir = RoomScript.Direction.LEFT;
        else if (dragDirection == Vector2Int.right) dir = RoomScript.Direction.RIGHT;

        if (!currentRoom.CanMove(dir))
        {
            Debug.Log("Wall in that direction!");
            return;
        }

        Vector2Int nextCell = currentCell;
        switch (dir)
        {
            case RoomScript.Direction.TOP: nextCell.y += 1; break;
            case RoomScript.Direction.BOTTOM: nextCell.y -= 1; break;
            case RoomScript.Direction.LEFT: nextCell.x -= 1; break;
            case RoomScript.Direction.RIGHT: nextCell.x += 1; break;
        }

        if (nextCell.x < 0 || nextCell.x >= CE.numX || nextCell.y < 0 || nextCell.y >= CE.numY)
            return;

        RoomScript nextRoom = CE.rooms[nextCell.x, nextCell.y];
        transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, -1f);
        currentCell = nextCell;

        // Move player visually
        transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, -1f);
        currentCell = nextCell;

        moveCount++;
        Debug.Log($"Moved to room {currentCell}");

        CE.OnPlayerMove();

        CheckForCollectiblePickup();
    }

    private void CheckForCollectiblePickup()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Collectibles"))
            {
                Debug.Log($"Picked up {hit.name}");
                Destroy(hit.gameObject);
                CE.CollectibleCollected(hit.name);
                break;
            }
        }
    }
}
