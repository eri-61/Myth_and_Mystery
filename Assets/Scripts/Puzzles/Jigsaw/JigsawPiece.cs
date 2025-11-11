using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;          // <-- add this line


[RequireComponent(typeof(Image))]
public class JigsawPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    [HideInInspector] public JigsawManager manager;
    [HideInInspector] public Vector2 targetLocalPosition;
    [HideInInspector] public float snapDistance = 40f;

    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup cg;
    private Vector2 offset;
    private bool placed = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // record offset for nicer dragging
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        offset = (Vector2)rect.localPosition - localPoint;

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (placed) return;
        cg.blocksRaycasts = false;
        rect.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (placed) return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            rect.localPosition = localPoint + offset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (placed) return;
        cg.blocksRaycasts = true;

        // check snap: distance between current localPosition and targetLocalPosition
        float dist = Vector2.Distance(rect.localPosition, targetLocalPosition);
        if (dist <= snapDistance)
        {
            // snap into place
            rect.localPosition = targetLocalPosition;
            placed = true;
            // disable dragging visually
            cg.alpha = 1f;
            cg.blocksRaycasts = false; // won't be dragged again
            // optionally disable raycast target of Image to avoid UI blocking
            var image = GetComponent<Image>();
            image.raycastTarget = false;

            // Inform manager
            if (manager != null) manager.OnPiecePlaced();
        }
    }
}
