using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingGameManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private Transform gameTransform;
    [SerializeField] private Transform piecePrefab;
    [SerializeField] private LayerMask puzzleLayer;
    [SerializeField] private float interactDistance = 5f;

    private List<Transform> pieces;
    private int emptyLocation;
    private int width = 3;
    private int height = 3;
    private bool shuffling = false;
    private bool puzzleCompleted = false;
    private bool puzzleInitialized = false;      // ✅ new flag

    private Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
        if (mainCam == null)
            Debug.LogError("No MainCamera found! Tag your Cinemachine camera 'MainCamera'.");
    }

    private void Start()
    {
        pieces = new List<Transform>();
        CreateGamePieces(0.01f);
        StartCoroutine(InitializePuzzle());
    }

    private IEnumerator InitializePuzzle()
    {
        yield return new WaitForSeconds(0.3f); // small delay so meshes/colliders exist
        Shuffle();                              // ✅ shuffle first
        puzzleInitialized = true;               // ✅ allow completion checks afterwards
    }

    private void Update()
    {
        if (!puzzleInitialized || puzzleCompleted) return;
        if (mainCam == null) return;

        if (!shuffling && CheckCompletion())
        {
            Debug.Log("✅ Puzzle Completed!");
            puzzleCompleted = true;
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, puzzleLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.green, 0.25f);
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == hit.transform)
                    {
                        TryMovePiece(i);
                        break;
                    }
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red, 0.25f);
            }
        }
    }

    // ---------- core logic ----------

    private void TryMovePiece(int index)
    {
        if (SwapIfValid(index, -width)) return;      // up
        if (SwapIfValid(index, +width)) return;      // down
        if (index % width != 0 && SwapIfValid(index, -1)) return;          // left
        if (index % width != width - 1 && SwapIfValid(index, +1)) return;  // right
    }

    private bool SwapIfValid(int i, int offset)
    {
        int target = i + offset;
        if (target < 0 || target >= pieces.Count) return false;
        if (target != emptyLocation) return false;

        Vector3 temp = pieces[i].localPosition;
        pieces[i].localPosition = pieces[target].localPosition;
        pieces[target].localPosition = temp;

        (pieces[i], pieces[target]) = (pieces[target], pieces[i]);
        emptyLocation = i;
        return true;
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

    // ---------- generation & shuffle ----------

    private void CreateGamePieces(float gapThickness)
    {
        float tileWidth = 1f / width;
        float tileHeight = 1f / height;

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                Transform piece = Instantiate(piecePrefab, gameTransform);
                pieces.Add(piece);

                piece.localPosition = new Vector3(
                    -1 + (2 * tileWidth * col) + tileWidth,
                    +1 - (2 * tileHeight * row) - tileHeight,
                    0
                );

                piece.localScale = new Vector3((2 * tileWidth) - gapThickness,
                                               (2 * tileHeight) - gapThickness,
                                               1f);
                piece.name = $"{(row * width) + col}";

                if (row == height - 1 && col == width - 1)
                {
                    emptyLocation = (width * height) - 1;
                    piece.gameObject.SetActive(false);
                }
                else
                {
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

    private void Shuffle()
    {
        shuffling = true;
        int count = 0;
        int lastEmpty = emptyLocation;

        while (count < (width * height * height))
        {
            int rnd = Random.Range(0, width * height);
            if (rnd == lastEmpty) continue;
            lastEmpty = emptyLocation;

            if (SwapIfValid(rnd, -width)) { count++; continue; }
            if (SwapIfValid(rnd, +width)) { count++; continue; }
            if (rnd % width != 0 && SwapIfValid(rnd, -1)) { count++; continue; }
            if (rnd % width != width - 1 && SwapIfValid(rnd, +1)) { count++; continue; }
        }

        shuffling = false;
    }
}
