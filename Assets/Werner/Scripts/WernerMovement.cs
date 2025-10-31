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

    private Camera playerCam;

    private Vector3 moveDirection;
    private Vector3 yVelocity;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 8.0f;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Gravity")]
    [SerializeField] private float gravityValue = -9.81f;

    [SerializeField] private Vector3 standCameraOffset = new Vector3(0, 1.7f, 0);

    [Header("Head Bobbing")]
    [SerializeField] private float walkBobAmplitude = 0.04f;
    [SerializeField] private float walkBobFrequency = 6f;
    [SerializeField] private float sprintBobAmplitude = 0.08f;
    [SerializeField] private float sprintBobFrequency = 10f;

    [Header("Head Sway (horizontal)")]
    [SerializeField] private float walkSwayAmplitude = 0.02f;
    [SerializeField] private float sprintSwayAmplitude = 0.04f;
    [SerializeField] private float swayFrequencyMultiplier = 0.5f;

    private bool groundedPlayer = true;
    private bool isJumping = false;

    private float bobTimer;
    private Vector3 baseCamPos;
    private Vector3 currentCameraOffset;

    private CollisionFlags collisionFlags; // ✅ added to track ceiling hits

    public bool OnGround => groundedPlayer;
    public Transform CameraTarget => cameraTarget;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        groundCheck = GetComponentInChildren<GroundCheck>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        playerCam = Camera.main ?? GetComponentInChildren<Camera>();
        if (playerCam == null)
            Debug.LogWarning("WernerMovement: No camera found on player!");

        if (cameraTarget != null)
        {
            cameraTarget.localPosition = standCameraOffset;
            baseCamPos = standCameraOffset;
            currentCameraOffset = standCameraOffset;
        }
    }

    private void Update()
    {
        groundedPlayer = controller.isGrounded || (groundCheck != null && groundCheck.OnGround());

        // ✅ If interacting, still apply gravity but stop other inputs
        if (GameManager.instance != null && GameManager.instance.CurrentState != InteractionState.None)
        {
            ApplyGravityOnly();
            StopAnimationsWhileInteracting();
            return;
        }

        HandleMovementAndGravity();
        HandleHeadBobAndSway();
        HandleAnimations();
    }

    private void ApplyGravityOnly()
    {
        if (groundedPlayer && yVelocity.y < 0f)
        {
            yVelocity.y = -2f;
        }

        yVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(yVelocity * Time.deltaTime);
    }

    private void StopAnimationsWhileInteracting()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsJumping", false);
        }
    }

    private void HandleMovementAndGravity()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed;

        if (playerCam == null)
        {
            playerCam = Camera.main ?? GetComponentInChildren<Camera>();
            if (playerCam == null)
                return;
        }

        Vector3 camForward = playerCam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        if (camForward.sqrMagnitude < 0.01f)
            camForward = transform.forward;

        Vector3 camRight = playerCam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 move = camForward * inputZ + camRight * inputX;
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

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            yVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravityValue);
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }

        yVelocity.y += gravityValue * Time.deltaTime;
        Vector3 finalVelocity = moveVelocity + yVelocity;

        // ✅ Move controller and detect collisions
        collisionFlags = controller.Move(finalVelocity * Time.deltaTime);

        // ✅ Instantly cancel upward velocity if we hit a ceiling
        if ((collisionFlags & CollisionFlags.Above) != 0 && yVelocity.y > 0f)
        {
            yVelocity.y = -2f; // small downward push to resume falling
        }

        moveDirection = move;
    }

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

        float amplitude = Input.GetKey(KeyCode.LeftShift) ? sprintBobAmplitude : walkBobAmplitude;
        float frequency = Input.GetKey(KeyCode.LeftShift) ? sprintBobFrequency : walkBobFrequency;
        float swayAmp = Input.GetKey(KeyCode.LeftShift) ? sprintSwayAmplitude : walkSwayAmplitude;

        bobTimer += Time.deltaTime * frequency;

        float offsetY = Mathf.Sin(bobTimer) * amplitude;
        float offsetX = Mathf.Sin(bobTimer * swayFrequencyMultiplier + Mathf.PI / 2f) * swayAmp;

        cameraTarget.localPosition = baseCamPos + new Vector3(offsetX, offsetY, 0);
    }

    private void HandleAnimations()
    {
        if (animator == null) return;
        if (isJumping) return;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        Vector2 inputVector = new Vector2(inputX, inputZ);
        float inputMagnitude = Mathf.Clamp01(inputVector.magnitude);

        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && inputMagnitude > 0.1f;

        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
        float normalizedSpeed = Mathf.InverseLerp(0f, sprintSpeed, inputMagnitude * targetSpeed);

        animator.SetFloat("Speed", normalizedSpeed, 0.1f, Time.deltaTime);
    }

    public void RefreshCamera()
    {
        playerCam = Camera.main ?? GetComponentInChildren<Camera>();

        if (playerCam == null)
        {
            return;
        }

        if (cameraTarget != null)
        {
            cameraTarget.localPosition = standCameraOffset;
            baseCamPos = standCameraOffset;
            currentCameraOffset = standCameraOffset;
        }
    }

    public void TeleportTo(Vector3 position, Quaternion rotation)
    {
        if (controller != null)
            controller.enabled = false;

        transform.position = position;
        transform.rotation = rotation;

        // Reset gravity / jump velocity
        yVelocity = Vector3.zero;

        if (controller != null)
            controller.enabled = true;
    }
}
