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

    private bool needsBobReset = false;

    private CollisionFlags collisionFlags;

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
        }
    }

    private void Update()
    {
        groundedPlayer = controller.isGrounded || (groundCheck != null && groundCheck.OnGround());

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
            if (playerCam == null) return;
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

        collisionFlags = controller.Move(finalVelocity * Time.deltaTime);

        if ((collisionFlags & CollisionFlags.Above) != 0 && yVelocity.y > 0f)
        {
            yVelocity.y = -2f;
        }

        moveDirection = move;
    }

    private void HandleHeadBobAndSway()
    {
        if (cameraTarget == null || !groundedPlayer) return;

        float moveInput = Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical"));
        bool isMoving = moveInput > 0.1f;

        // ✅ If we just teleported, reset bob only when player walks again
        if (needsBobReset)
        {
            if (isMoving)
            {
                needsBobReset = false;
                bobTimer = 0f;
                cameraTarget.localPosition = standCameraOffset;
                baseCamPos = standCameraOffset;
            }
            else
            {
                return; // Don't bob until movement starts again
            }
        }

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

    public void OnTeleportedSnap()
    {
        StartCoroutine(TeleportFixRoutine());
    }

    private IEnumerator TeleportFixRoutine()
    {
        // ✅ Wait 1 frame so CharacterController + position update properly
        yield return null;

        // ✅ Reset controller and re-ground
        if (controller != null)
        {
            controller.enabled = false;
            yVelocity = Vector3.zero;
            controller.enabled = true;
            controller.Move(Vector3.down * 0.2f);
        }

        // ✅ Reset camera target but don’t start bobbing yet
        if (cameraTarget != null)
        {
            cameraTarget.localPosition = standCameraOffset;
            baseCamPos = standCameraOffset;
            bobTimer = 0f;
        }

        // ✅ Wait for player to move before bob returns
        needsBobReset = true;
    }


    public void TeleportTo(Vector3 position, Quaternion rotation)
    {
        if (controller != null)
            controller.enabled = false;

        transform.position = position;
        transform.rotation = rotation;

        // Reset gravity / jump velocity so teleport doesn't cause weird floating
        yVelocity = Vector3.zero;

        if (controller != null)
            controller.enabled = true;

        // Ensure head bob is reset
        needsBobReset = true;
        bobTimer = 0;
    }

    // ✅ Keeps compatibility with RiftTravel scripts that still call RefreshCamera()
    public void RefreshCamera()
    {
        if (cameraTarget != null)
        {
            cameraTarget.localPosition = standCameraOffset;
            baseCamPos = standCameraOffset;
            bobTimer = 0f;
        }
    }
    
    public void ForceHeadBobReset()
    {
        // Kept for backward compatibility
        needsBobReset = true;
        bobTimer = 0f;

        if (cameraTarget != null)
        {
            cameraTarget.localPosition = standCameraOffset;
            baseCamPos = standCameraOffset;
        }
    }
}
