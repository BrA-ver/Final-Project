using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    InputHandler input;
    PlayerMovement movement;
    Animator animator;
    Interactor interactor;

    int moving = Animator.StringToHash("moving");
    int grounded = Animator.StringToHash("onGround");

    public bool waitForNextFrame = true;

    Vector3 startPos;

    private void Awake()
    {
        input = GetComponent<InputHandler>();
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        interactor = GetComponentInChildren<Interactor>();
    }

    private void Start()
    {
        waitForNextFrame = true;
        //movement.enabled = true;

        //transform.position = startPos;
    }

    private void OnEnable()
    {
        input.onSumbit += OnJump;
        input.onInteract += OnInteract;
        input.onPresentEvidence += OnPresentEvidence;
        input.onCaseBoard += OnCaseBoard;
    }

    private void OnDisable()
    {
        input.onJump -= OnJump;
        input.onInteract -= OnInteract;
        input.onPresentEvidence -= OnPresentEvidence;
        input.onCaseBoard += OnCaseBoard;
    }

    

    private void Update()
    {
        if (GameManager.instance.CurrentState != InteractionState.None)
        {
            movement.Stop();
            animator.SetBool(moving, false);
            return;
        }

        HandleMovement();
    }

    void AnimateMovement()
    {
        //bool isMoving = movement.Velocity.magnitude > 0.1f;
        //animator.SetBool(moving, isMoving);

        //animator.SetBool(grounded, movement.OnGround);
    }

    private void HandleMovement()
    {
        if (waitForNextFrame)
        {
            waitForNextFrame = false;
        }
        else
        {
            Vector2 moveInput = input.moveInput;
            movement.Move(moveInput);
        }

        AnimateMovement();
    }

    void OnJump()
    {
        if (waitForNextFrame || GameManager.instance.IsInteracting) return;
        //Debug.Log("Jump Called");
        movement.Jump();
    }

    public void SetPosition(Vector3 newPos)
    {
        startPos = newPos;
    }

    private void OnInteract()
    {
        interactor.Interact();
    }

    private void OnPresentEvidence()
    {
        EvidenceDisplay.instance.ToggleEvidence();
    }

    void OnCaseBoard()
    {
        InteractionState currentState = GameManager.instance.CurrentState;
        // Only toggle the case file if we are free or in the case file
        if (currentState != InteractionState.None && currentState != InteractionState.CaseFile)
        {
            return;
        }

        CaseFile file = CaseFile.instance;
        if (file.IsOpen)
        {
            file.CloseCaseFile();
        }
        else
        {
            file.OpenCaseFile();
        }
    }
}
