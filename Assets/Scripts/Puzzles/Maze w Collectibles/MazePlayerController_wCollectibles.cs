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

    private Rigidbody2D rb;

    private void Awake()
    {
        CE = FindAnyObjectByType<GenerateMaze_wCollectibles>();
    }

    private void Start()
    {
        cam = Camera.main;

        // Ensure collider for physics / triggers
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            BoxCollider2D box = gameObject.AddComponent<BoxCollider2D>();
            box.isTrigger = false; // not a trigger — Exit uses a trigger collider
        }
        else
        {
            col.isTrigger = false;
        }

        // Ensure a Rigidbody2D exists. Use Kinematic so we can MovePosition and still get trigger callbacks.
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }

        // ensure the player GameObject has the "Player" tag so ExitZoneTrigger sees it
        try
        {
            gameObject.tag = "Player";
        }
        catch { /* ignore if tag missing */ }
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

        // when movement would leave the grid, attempt exit via physics move
        if (nextCell.x < 0 || nextCell.x >= CE.numX || nextCell.y < 0 || nextCell.y >= CE.numY) return;

        RoomScript nextRoom = CE.rooms[nextCell.x, nextCell.y];

        // Move via Rigidbody2D to keep physics consistent
        if (rb != null)
        {
            Vector2 target = new Vector2(nextRoom.transform.position.x, nextRoom.transform.position.y);
            rb.MovePosition(target);
            // ensure z ordering
            transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
        }
        else
        {
            transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, -1f);
        }

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
