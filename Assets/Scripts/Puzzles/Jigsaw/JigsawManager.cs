using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JigsawManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform canvasRect;         // Canvas RectTransform
    public RectTransform targetArea;         // Where final puzzle sits
    public GameObject piecePrefab;           // UI Image prefab with JigsawPiece.cs

    [Header("Puzzle Sprites")]
    public List<Sprite> puzzleSprites;       // Assign your sliced images here

    [Header("Grid")]
    public int rows = 3;
    public int cols = 3;

    [Header("Scatter Settings")]
    public float scatterMargin = 50f;
    public float minScatterDistance = 50f;

    [Header("Snap Settings")]
    public float snapDistance = 40f;

    private Vector2 targetSize;
    private List<GameObject> pieces = new List<GameObject>();
    private int placedCount = 0;

    void Start()
    {
        if (canvasRect == null) canvasRect = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        if (piecePrefab == null) { Debug.LogError("Assign piecePrefab in inspector."); return; }
        if (targetArea == null) { Debug.LogError("Assign targetArea in inspector."); return; }
        if (puzzleSprites.Count == 0) { Debug.LogError("Assign sliced puzzle images in inspector."); return; }

        CreatePieces();
        ScatterPieces();
    }

    void CreatePieces()
    {
        float tw = targetArea.rect.width;
        float th = targetArea.rect.height;
        targetSize = new Vector2(tw / cols, th / rows);

        int index = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (index >= puzzleSprites.Count) break;

                Sprite pieceSprite = puzzleSprites[index];
                index++;

                GameObject g = Instantiate(piecePrefab, canvasRect);
                g.name = $"Piece_{r}_{c}";

                Image img = g.GetComponent<Image>();
                img.sprite = pieceSprite;
                img.preserveAspect = false; // ✅ ensures no gaps
                img.rectTransform.sizeDelta = targetSize;

                Vector2 localPos = GetTargetLocalPosition(r, c);

                var pieceComp = g.GetComponent<JigsawPiece>();
                pieceComp.manager = this;
                pieceComp.targetLocalPosition = localPos;
                pieceComp.snapDistance = snapDistance;

                pieces.Add(g);
            }
        }
    }

    Vector2 GetTargetLocalPosition(int row, int col)
    {
        float startX = targetArea.localPosition.x - (targetArea.rect.width / 2f) + (targetSize.x / 2f);
        float startY = targetArea.localPosition.y + (targetArea.rect.height / 2f) - (targetSize.y / 2f);
        float x = startX + col * targetSize.x;
        float y = startY - row * targetSize.y;
        return new Vector2(x, y);
    }

    void ScatterPieces()
    {
        Rect screenRect = canvasRect.rect;

        foreach (var g in pieces)
        {
            RectTransform rt = g.GetComponent<RectTransform>();

            Vector2 pos;
            int safety = 0;
            do
            {
                float x = Random.Range(screenRect.xMin + scatterMargin, screenRect.xMax - scatterMargin);
                float y = Random.Range(screenRect.yMin + scatterMargin, screenRect.yMax - scatterMargin);
                pos = new Vector2(x, y);
                safety++;
                if (safety > 200) break;
            }
            while (Vector2.Distance(pos, targetArea.localPosition) < minScatterDistance);

            rt.localPosition = pos;
            rt.SetAsLastSibling();
        }
    }

    public void OnPiecePlaced()
    {
        placedCount++;
        if (placedCount >= pieces.Count)
        {
            Debug.Log("✅ Puzzle complete!");
        }
    }
}
