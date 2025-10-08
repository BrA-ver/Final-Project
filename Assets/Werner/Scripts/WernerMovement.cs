using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class WernerMovement : MonoBehaviour
{
    private CharacterController controller;

    [Header("References")]
    [SerializeField] private Animator animator; 
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private Transform cameraTarget;

    private Vector3 moveDirection;
    private Vector3 yVelocity;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 8.0f;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Gravity")]
    [SerializeField] private float gravityValue = -9.81f;

    // CROUCH SETTINGS (commented out except standCameraOffset)
    /*
    [Header("Crouch Settings")]
    [SerializeField] private float standHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchTransitionSpeed = 8f;
    */
    [SerializeField] private Vector3 standCameraOffset = new Vector3(0, 1.7f, 0);
    /*
    [SerializeField] private Vector3 crouchCameraOffset = new Vector3(0, 1.0f, 0);
    [SerializeField] private bool holdToCrouch = false;
    */

    [Header("Head Bobbing")]
    [SerializeField] private float walkBobAmplitude = 0.04f;
    [SerializeField] private float walkBobFrequency = 6f;
    [SerializeField] private float sprintBobAmplitude = 0.08f;
    [SerializeField] private float sprintBobFrequency = 10f;
    // [SerializeField] private float crouchBobAmplitude = 0.02f;
    // [SerializeField] private float crouchBobFrequency = 4f;

    [Header("Head Sway (horizontal)")]
    [SerializeField] private float walkSwayAmplitude = 0.02f;
    [SerializeField] private float sprintSwayAmplitude = 0.04f;
    // [SerializeField] private float crouchSwayAmplitude = 0.01f;
    [SerializeField] private float swayFrequencyMultiplier = 0.5f;

    private bool groundedPlayer = true;
    // private bool isCrouching = false;
    private bool isJumping = false;
    // private bool isTransitioningCrouch = false;

    private float bobTimer;
    private Vector3 baseCamPos;
    private Vector3 currentCameraOffset;

    public bool OnGround => groundedPlayer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        groundCheck = GetComponentInChildren<GroundCheck>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (cameraTarget != null)
        {
            cameraTarget.localPosition = standCameraOffset;
            baseCamPos = standCameraOffset;
            currentCameraOffset = standCameraOffset;
        }
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.CurrentState != InteractionState.None)
            return;

        groundedPlayer = controller.isGrounded || (groundCheck != null && groundCheck.OnGround());

        // HandleCrouchInput();
        HandleMovementAndGravity();
        // UpdateCrouchHeightSmooth();
        HandleHeadBobAndSway();
        HandleAnimations();
    }

    private void HandleMovementAndGravity()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        float currentSpeed = moveSpeed;

        // if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = sprintSpeed;
        // else if (isCrouching)
        //     currentSpeed = crouchSpeed;

        Vector3 move = Camera.main.transform.forward * inputZ + Camera.main.transform.right * inputX;
        move.y = 0f;
        move.Normalize();

        Vector3 moveVelocity = move * currentSpeed;

        if (groundedPlayer && yVelocity.y < 0f)
        {
            yVelocity.y = -2f;

            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("IsJumping", false);
            }
        }

        if (Input.GetButtonDown("Jump") && groundedPlayer /* && !isCrouching */)
        {
            yVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravityValue);
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }

        yVelocity.y += gravityValue * Time.deltaTime;
        Vector3 finalVelocity = moveVelocity + yVelocity;
        controller.Move(finalVelocity * Time.deltaTime);
        moveDirection = move;
    }

    // CROUCH FUNCTIONS REMOVED
    /*
    private void HandleCrouchInput() { ... }
    private IEnumerator SmoothCrouchTransition(...) { ... }
    private float GetAnimationLength(string name) { ... }
    private void UpdateCrouchHeightSmooth() { ... }
    */

    private void HandleHeadBobAndSway()
    {
        if (cameraTarget == null || !groundedPlayer) return;

        float moveInput = Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical"));
        bool isMoving = moveInput > 0.1f;

        if (!isMoving)
        {
            bobTimer = 0f;
            cameraTarget.localPosition = Vector3.Lerp(cameraTarget.localPosition, baseCamPos, Time.deltaTime * 5f);
            return;
        }

        float amplitude =
            // isCrouching ? crouchBobAmplitude :
            Input.GetKey(KeyCode.LeftShift) ? sprintBobAmplitude : walkBobAmplitude;

        float frequency =
            // isCrouching ? crouchBobFrequency :
            Input.GetKey(KeyCode.LeftShift) ? sprintBobFrequency : walkBobFrequency;

        float swayAmp =
            // isCrouching ? crouchSwayAmplitude :
            Input.GetKey(KeyCode.LeftShift) ? sprintSwayAmplitude : walkSwayAmplitude;

        bobTimer += Time.deltaTime * frequency;

        float offsetY = Mathf.Sin(bobTimer) * amplitude;
        float offsetX = Mathf.Sin(bobTimer * swayFrequencyMultiplier + Mathf.PI / 2f) * swayAmp;

        cameraTarget.localPosition = baseCamPos + new Vector3(offsetX, offsetY, 0);
    }

    private void HandleAnimations()
    {
        if (animator == null) return;
        // if (isJumping || isTransitioningCrouch) return;
        if (isJumping) return;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        Vector2 inputVector = new Vector2(inputX, inputZ);
        float inputMagnitude = Mathf.Clamp01(inputVector.magnitude);

        // bool isSprinting = Input.GetKey(KeyCode.LeftShift) && inputMagnitude > 0.1f && !isCrouching;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && inputMagnitude > 0.1f;

        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
        float normalizedSpeed = Mathf.InverseLerp(0f, sprintSpeed, inputMagnitude * targetSpeed);

        animator.SetFloat("Speed", normalizedSpeed, 0.1f, Time.deltaTime);
    }
}
