using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Required for IEnumerator and Coroutines

public class Puzzle4 : MonoBehaviour
{
    [SerializeField] private Transform gameTransform;
    [SerializeField] private Transform piecePrefab;

    private List<Transform> pieces;
    private int emptyLocation;
    private int size;
    private bool shuffling = false;

    //Create the game setup with size x size pieces.
    private void CreateGamePieces(float gapThickness)
    {
        //This is the width each tile.
        float width = 1 / (float)size;
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                Transform piece = Instantiate(piecePrefab, gameTransform);
                pieces.Add(piece);


                // X-position: from -1 (left) to 1 (right)
                float x = -1f + (2 * width * col) + width;
                // Y-position: from 1 (top) to -1 (bottom)
                float y = 1f - (2 * width * row) - width;
                piece.localPosition = new Vector3(x, y, 0);


                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                piece.name = $"{(row * size) + col}";
                //We want an empty space in the bottom right.
                if ((row == size - 1) && (col == size - 1))
                {
                    emptyLocation = (size * size) - 1;
                    piece.gameObject.SetActive(false);

                }
                else
                {
                    //We want to map the UV coordinates appropriately, they are 0->1
                    float gap = gapThickness / 2;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];

                    //UV coord order (0,1), (1,1), (0,0), (1,0)

                    uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
                    uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
                    uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
                    uv[3] = new Vector2((width * (col + 1)) - gap, 1 - ((width * row) + gap));
                    //Assign new UVs to the mesh
                    mesh.uv = uv;
                }
            }
        }
    }


    void Start()
    {
        pieces = new List<Transform>();
        size = 5;
        CreateGamePieces(0.01f);
    }


    //Update is called once per frame

    void Update()
    {

        if (!shuffling && CheckCompletion())
        {
            shuffling = true;
            StartCoroutine(WaitShuffle(0.5f));
        }

        //On click send out ray to see if we click a piece
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                //Go through the list, the index tells us the position
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == hit.transform)
                    {
                        // Check if the clicked piece can swap with the empty location (emptyLocation is an index)
                        // size is for up/down checks, 0 and size-1 for left/right boundary checks
                        if (SwapIfValid(i, -size, size)) { break; }
                        if (SwapIfValid(i, +size, size)) { break; }
                        if (SwapIfValid(i, -1, 0)) { break; }
                        if (SwapIfValid(i, +1, size - 1)) { break; }
                    }
                }
            }
        }
    }

    private bool CheckCompletion()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].name != $"{i}")
            {
                return false;
            }
        }
        return true;
    }


    private IEnumerator WaitShuffle(float duration)
    {
        yield return new WaitForSeconds(duration);
        Shuffle();
        shuffling = false;
    }

    private void Shuffle()
    {
        int count = 0;
        int last = 0;

        while (count < size * size * size)
        {
            //Pick random location
            int rnd = Random.Range(0, size * size);

            if (rnd == last) { continue; }
            last = emptyLocation;

            if (SwapIfValid(rnd, -size, size))
            {
                count++;
            }
            else if (SwapIfValid(rnd, +size, size))
            {
                count++;
            }
            else if (SwapIfValid(rnd, -1, 0))
            {
                count++;
            }
            else if (SwapIfValid(rnd, +1, size - 1))
            {
                count++;
            }
        }
    }


    // This method block was previously outside the class scope
    private bool SwapIfValid(int i, int offset, int colCheck)
    {
        // Check 1: Ensure the move is not across rows (for -1 and +1 moves). colCheck is the column boundary to prevent wrapping.
        // Check 2: Ensure the target slot (i + offset) is the empty location.
        if (((i % size) != colCheck) && ((i + offset) == emptyLocation))
        {
            // Swap them in game state (list indices)
            (pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);

            // Swap their world positions. Note the correct C# 7 tuple swap syntax.
            (pieces[i].localPosition, pieces[i + offset].localPosition) = (pieces[i + offset].localPosition, pieces[i].localPosition);

            // Update empty location to the index of the piece that was just clicked
            emptyLocation = i;
            return true;
        }

        return false;
    }
}