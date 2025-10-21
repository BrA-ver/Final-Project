using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    InputHandler input;
    WernerMovement movement;
    Animator animator;
    Interactor interactor;

    int moving = Animator.StringToHash("moving");
    int grounded = Animator.StringToHash("onGround");

    public bool waitForNextFrame = true;

    Vector3 startPos;

    public CharacterController Controller { get; private set; }

    private void Awake()
    {
        input = GetComponent<InputHandler>();
        movement = GetComponent<WernerMovement>();
        animator = GetComponent<Animator>();
        interactor = GetComponentInChildren<Interactor>();
        Controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        waitForNextFrame = true;
    }

    private void OnEnable()
    {
        input.onInteract += OnInteract;
        input.onPresentEvidence += OnPresentEvidence;
        input.onCaseBoard += OnCaseBoard;
    }

    private void OnDisable()
    {
        input.onInteract -= OnInteract;
        input.onPresentEvidence -= OnPresentEvidence;
        input.onCaseBoard -= OnCaseBoard;
    }

    private void Update()
    {
        if (GameManager.instance != null && GameManager.instance.CurrentState != InteractionState.None)
        {
            return;
        }

        AnimateMovement();
    }

    void AnimateMovement()
    {
        if (animator == null) return;
        bool isGrounded = movement != null && movement.OnGround;
        animator.SetBool(grounded, isGrounded);
    }

    public void SetPosition(Vector3 newPos)
    {
        startPos = newPos;
    }

    private void OnInteract()
    {
        interactor?.Interact();
    }

    private void OnPresentEvidence()
    {
        ToggleEvidenceDisplay();
    }

    void ToggleEvidenceDisplay()
    {
        EvidenceDisplay.instance.ToggleEvidence();
    }

    void OnCaseBoard()
    {
        InteractionState currentState = GameManager.instance.CurrentState;
        if (currentState != InteractionState.None && currentState != InteractionState.CaseFile)
            return;

        CaseFile file = CaseFile.instance;
        if (file.IsOpen)
            file.CloseCaseFile();
        else
            file.OpenCaseFile();
    }
}
