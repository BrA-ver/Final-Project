using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    Vector3 moveDirection;
    Vector3 velocity;
    Vector3 yVelocity;
    
    [Header("Movevemnt")]
    [SerializeField] private float moveSpeed = 5.0f;

    [Header("Vertical Movement")]
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] private float jumpHeight = 1.5f;
    private float gravityValue = -9.81f;

    [Header("Rotation")]
    [SerializeField] float rotationSpeed = 5f;

    public Vector3 Velocity => velocity;
    public bool OnGround => groundedPlayer;

    
    private Vector3 playerVelocity;
    private bool groundedPlayer = true;

    private Vector2 movementInput = Vector2.zero;
    private bool jumped = false;

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void Update()
    {

        groundedPlayer = false;
        if (controller.isGrounded || groundCheck.OnGround())
            groundedPlayer = true;

        GroundMovement();
        VerticalMovement();
        HandleRotation();
    }

    void GroundMovement()
    {
        velocity = moveDirection * moveSpeed;
        controller.Move(velocity * Time.deltaTime);
    }

    void VerticalMovement()
    {
        if (groundedPlayer && yVelocity.y < 0f)
            yVelocity.y = 0f;
        else
            yVelocity.y += gravityValue * Time.deltaTime;

        controller.Move(yVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (groundedPlayer)
        {
            yVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravityValue);
        }
    }

    void HandleRotation()
    {
        if (moveDirection.magnitude < 0.1f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = rotation;
    }

    public void Move(Vector2 direction)
    {
        moveDirection = Camera.main.transform.forward * direction.y;
        moveDirection += Camera.main.transform.right * direction.x;
        moveDirection.y = 0f;
        moveDirection.Normalize();
    }

    public void Stop()
    {
        velocity = Vector3.zero;
        moveDirection = Vector3.zero;
    }
}
