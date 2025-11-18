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

    [Header("Auto Complete Settings")]
    [SerializeField] private int movesBeforeAutoComplete = 10;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip puzzleCompleteSFX;

    private List<Transform> pieces;
    private int emptyLocation;
    private int width = 3;
    private int height = 3;
    private bool shuffling = false;
    private bool puzzleCompleted = false;
    private bool puzzleInitialized = false;

    private Camera mainCam;

    private int moveCount = 0;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        pieces = new List<Transform>();
        CreateGamePieces(0.01f);
        StartCoroutine(InitializePuzzle());
    }

    private IEnumerator InitializePuzzle()
    {
        yield return new WaitForSeconds(0.3f);
        Shuffle();
        puzzleInitialized = true;
    }

    private void Update()
    {
        if (!puzzleInitialized || puzzleCompleted) return;
        if (mainCam == null) return;

        if (!shuffling && CheckCompletion())
        {
            CompletePuzzle();
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, puzzleLayer))
            {
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == hit.transform)
                    {
                        if (TryMovePiece(i))
                        {
                            moveCount++;

                            if (moveCount >= movesBeforeAutoComplete)
                            {
                                AutoCompletePuzzle();
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    private bool TryMovePiece(int index)
    {
        if (SwapIfValid(index, -width)) return true;
        if (SwapIfValid(index, +width)) return true;
        if (index % width != 0 && SwapIfValid(index, -1)) return true;
        if (index % width != width - 1 && SwapIfValid(index, +1)) return true;

        return false;
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

    private void AutoCompletePuzzle()
    {
        float tileWidth = 1f / width;
        float tileHeight = 1f / height;

        for (int i = 0; i < pieces.Count; i++)
        {
            int row = i / width;
            int col = i % width;

            pieces[i].localPosition = new Vector3(
                -1 + (2f * tileWidth * col) + tileWidth,
                +1 - (2f * tileHeight * row) - tileHeight,
                0
            );

            pieces[i].name = "" + i;

            if (pieces[i].gameObject.activeSelf)
            {
                Mesh mesh = pieces[i].GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[4];

                float gap = 0.005f;

                uv[0] = new Vector2((tileWidth * col) + gap,     1 - ((tileHeight * (row + 1)) - gap));
                uv[1] = new Vector2((tileWidth * (col + 1)) - gap, 1 - ((tileHeight * (row + 1)) - gap));
                uv[2] = new Vector2((tileWidth * col) + gap,     1 - ((tileHeight * row) + gap));
                uv[3] = new Vector2((tileWidth * (col + 1)) - gap, 1 - ((tileHeight * row) + gap));

                mesh.uv = uv;
            }
        }

        emptyLocation = width * height - 1;
        CompletePuzzle();
    }

    private void CompletePuzzle()
    {
        if (puzzleCompleted) return;

        puzzleCompleted = true;
        Debug.Log("🎉 Puzzle Completed!");

        if (audioSource != null && puzzleCompleteSFX != null)
            audioSource.PlayOneShot(puzzleCompleteSFX);
    }

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

                piece.localScale = new Vector3(
                    (2 * tileWidth) - gapThickness,
                    (2 * tileHeight) - gapThickness,
                    1f
                );

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
