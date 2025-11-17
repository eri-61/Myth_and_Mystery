using UnityEngine;
using UnityEngine.EventSystems; 

public class MazePlayerController : MonoBehaviour
{
    public GenerateMaze generateMaze;
    public Vector2Int currentCell;

    private Camera cam;
    private bool isDragging = false;
    private Vector3 dragStartWorldPos;
    private Vector2Int dragDirection = Vector2Int.zero;
    private Collider2D col;

    private void Awake()
    {
        generateMaze = FindAnyObjectByType<GenerateMaze>();
    }

    private void Start()
    {
        cam = Camera.main;
        if (cam == null)
        {
            cam = Camera.main;
            Debug.LogWarning("⚠️ Camera reference was null — reassigning Camera.main");
        }
        // Ensure collider exists
        col = GetComponent<Collider2D>();
        if (col == null)
        {
            BoxCollider2D box = gameObject.AddComponent<BoxCollider2D>();
            box.isTrigger = false; 
            col = box;
        }
    }

    private void Update()
    {
        // --- Input Start (mouse or touch) ---
        if (IsPointerDown())
        {
            Vector3 inputPos = GetInputWorldPosition();
            Vector2 inputPos2D = new Vector2(inputPos.x, inputPos.y);

            RaycastHit2D hit = Physics2D.Raycast(inputPos2D, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                dragStartWorldPos = inputPos;
                Debug.Log($"🟢 CLICKED PLAYER at {dragStartWorldPos}");
            }
            else
            {
                Debug.Log($"❌ Clicked something else: {hit.collider?.name ?? "nothing"}");
            }
        }

        // --- Input End (mouse up or touch end) ---
        if (isDragging && IsPointerUp())
        {
            isDragging = false;

            Vector3 dragEndWorldPos = GetInputWorldPosition();
            Vector3 dragVector = dragEndWorldPos - dragStartWorldPos;

            if (Mathf.Abs(dragVector.x) > Mathf.Abs(dragVector.y))
                dragDirection = dragVector.x > 0 ? Vector2Int.right : Vector2Int.left;
            else
                dragDirection = dragVector.y > 0 ? Vector2Int.up : Vector2Int.down;

            Debug.Log($"🔵 RELEASED at {dragEndWorldPos} — Direction: {dragDirection}");

            TryMoveToNextRoom();
        }
    }

    private bool IsPointerDown()
    {
        // Detect first touch or mouse down
        return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            || Input.GetMouseButtonDown(0);
    }

    private bool IsPointerUp()
    {
        // Detect touch end or mouse up
        return (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            || Input.GetMouseButtonUp(0);
    }

    private Vector3 GetInputWorldPosition()
    {
        Vector3 inputPos;

        if (Input.touchCount > 0)
            inputPos = Input.GetTouch(0).position;
        else
            inputPos = Input.mousePosition;

        Vector3 worldPos = cam.ScreenToWorldPoint(inputPos);
        worldPos.z = 0f; // ✅ Match player and maze plane
        return worldPos;
    }

    private void TryMoveToNextRoom()
    {
        if (generateMaze == null) return;

        RoomScript currentRoom = generateMaze.rooms[currentCell.x, currentCell.y];

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

        if (nextCell.x < 0 || nextCell.x >= generateMaze.numX || nextCell.y < 0 || nextCell.y >= generateMaze.numY)
            return;

        RoomScript nextRoom = generateMaze.rooms[nextCell.x, nextCell.y];

        // ✅ Keep everything on same Z plane for raycast consistency
        transform.position = new Vector3(nextRoom.transform.position.x, nextRoom.transform.position.y, 0f);
        currentCell = nextCell;

        Debug.Log($"Moved to room {currentCell}");
    }
}
