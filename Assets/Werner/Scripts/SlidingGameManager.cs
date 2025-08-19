using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingGameManager : MonoBehaviour
{
    [SerializeField] private Transform gameTransform;  // Parent for puzzle pieces
    [SerializeField] private Transform piecePrefab;    // Prefab (Quad with MeshRenderer + BoxCollider)

    private List<Transform> pieces;
    private int emptyLocation;
    private int width;   // horizontal (columns)
    private int height;  // vertical (rows)
    private bool shuffling = false;

    // Create the game setup with width x height pieces.
    private void CreateGamePieces(float gapThickness)
    {
        float tileWidth = 1f / (float)width;
        float tileHeight = 1f / (float)height;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                Transform piece = Instantiate(piecePrefab, gameTransform);
                pieces.Add(piece);

                // Position each piece in a grid from -1 to +1
                piece.localPosition = new Vector3(
                    -1 + (2 * tileWidth * col) + tileWidth,
                    +1 - (2 * tileHeight * row) - tileHeight,
                    0
                );

                piece.localScale = new Vector3((2 * tileWidth) - gapThickness, (2 * tileHeight) - gapThickness, 1f);
                piece.name = $"{(row * width) + col}";

                // Leave bottom-right slot empty
                if ((row == height - 1) && (col == width - 1))
                {
                    emptyLocation = (width * height) - 1;
                    piece.gameObject.SetActive(false);
                }
                else
                {
                    // Map UV coordinates appropriately (for texture slicing)
                    float gap = gapThickness / 2;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];

                    uv[0] = new Vector2((tileWidth * col) + gap, 1 - ((tileHeight * (row + 1)) - gap));
                    uv[1] = new Vector2((tileWidth * (col + 1)) - gap, 1 - ((tileHeight * (row + 1)) - gap));
                    uv[2] = new Vector2((tileWidth * col) + gap, 1 - ((tileHeight * row) + gap));
                    uv[3] = new Vector2((tileWidth * (col + 1)) - gap, 1 - ((tileHeight * row) + gap));

                    mesh.uv = uv;
                }
            }
        }
    }

    void Start()
    {
        pieces = new List<Transform>();
        width = 3;   // 3 columns
        height = 3;  // 3 rows (changed from 4)
        CreateGamePieces(0.01f);
    }

    void Update()
    {
        // Check for completion
        if (!shuffling && CheckCompletion())
        {
            shuffling = true;
            StartCoroutine(WaitShuffle(0.5f));
        }

        // Handle mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == hit.transform)
                    {
                        // Check each direction to see if valid move
                        if (SwapIfValid(i, -width, width)) { break; }
                        if (SwapIfValid(i, +width, width)) { break; }
                        if (SwapIfValid(i, -1, 0)) { break; }
                        if (SwapIfValid(i, +1, width - 1)) { break; }
                    }
                }
            }
        }
    }

    private bool SwapIfValid(int i, int offset, int colCheck)
    {
        if (((i % width) != colCheck) && ((i + offset) == emptyLocation))
        {
            (pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);
            (pieces[i].localPosition, pieces[i + offset].localPosition) =
                (pieces[i + offset].localPosition, pieces[i].localPosition);
            emptyLocation = i;
            return true;
        }
        return false;
    }

    private bool CheckCompletion()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].name != $"{i}")
                return false;
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

        while (count < (width * height * height))
        {
            int rnd = Random.Range(0, width * height);
            if (rnd == last) { continue; }
            last = emptyLocation;

            if (SwapIfValid(rnd, -width, width)) { count++; }
            else if (SwapIfValid(rnd, +width, width)) { count++; }
            else if (SwapIfValid(rnd, -1, 0)) { count++; }
            else if (SwapIfValid(rnd, +1, width - 1)) { count++; }
        }
    }
}
