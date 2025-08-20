using UnityEngine;

public class Player : MonoBehaviour
{
    InputHandler input;
    PlayerMovement movement;
    Animator animator;
    int moving = Animator.StringToHash("moving");
    int grounded = Animator.StringToHash("onGround");

    public bool waitForNextFrame = true;

    Vector3 startPos;

    private void Awake()
    {
        input = GetComponent<InputHandler>();
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        waitForNextFrame = true;
        //movement.enabled = true;

        //transform.position = startPos;
    }

    private void OnEnable()
    {
        input.onJump += OnJump;
    }

    private void Update()
    {
        HandleMovement();
    }

    void AnimateMovement()
    {
        bool isMoving = movement.Velocity.magnitude > 0.1f;
        animator.SetBool(moving, isMoving);

        animator.SetBool(grounded, movement.OnGround);
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
        if (waitForNextFrame) return;
        movement.Jump();
    }

    public void SetPosition(Vector3 newPos)
    {
        startPos = newPos;
    }
}
