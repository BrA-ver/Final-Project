using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WernerMovement : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 moveDirection;
    private Vector3 yVelocity; // only for vertical motion

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 8.0f;

    [Header("Vertical Movement")]
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private float jumpHeight = 1.5f;
    private float gravityValue = -9.81f;

    public bool OnGround => groundedPlayer;
    private bool groundedPlayer = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded || groundCheck.OnGround();

        HandleMovement();
        HandleGravity();
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal"); // Raw gives instant stop
        float inputZ = Input.GetAxisRaw("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        // Camera-relative movement
        Vector3 move = Camera.main.transform.forward * inputZ + Camera.main.transform.right * inputX;
        move.y = 0f;
        move.Normalize();

        // Only horizontal here
        Vector3 horizontalVelocity = move * currentSpeed;

        controller.Move(horizontalVelocity * Time.deltaTime);

        // Jump
        if (groundedPlayer && Input.GetButtonDown("Jump"))
        {
            yVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravityValue);
        }
    }

    private void HandleGravity()
    {
        if (groundedPlayer && yVelocity.y < 0f)
        {
            yVelocity.y = -2f;
        }

        yVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(yVelocity * Time.deltaTime);
    }
}
