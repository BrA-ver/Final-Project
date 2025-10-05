using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WernerMovement : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 moveDirection;
    private Vector3 yVelocity;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 8.0f;

    [Header("Vertical Movement")]
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private float jumpHeight = 1.5f;

    [Header("Gravity")]
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2.0f;

    [Header("Crouch Settings")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float standHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchTransitionSpeed = 8f;
    [SerializeField] private Vector3 standCameraOffset = new Vector3(0, 1.7f, 0);
    [SerializeField] private Vector3 crouchCameraOffset = new Vector3(0, 1.0f, 0);
    [SerializeField] private bool holdToCrouch = false;

    [Header("Head Bobbing")]
    [SerializeField] private float walkBobAmplitude = 0.04f;
    [SerializeField] private float walkBobFrequency = 6f;
    [SerializeField] private float sprintBobAmplitude = 0.08f;
    [SerializeField] private float sprintBobFrequency = 10f;
    [SerializeField] private float crouchBobAmplitude = 0.02f;
    [SerializeField] private float crouchBobFrequency = 4f;

    [Header("Head Sway (horizontal)")]
    [SerializeField] private float walkSwayAmplitude = 0.02f;
    [SerializeField] private float sprintSwayAmplitude = 0.04f;
    [SerializeField] private float crouchSwayAmplitude = 0.01f;
    [SerializeField] private float swayFrequencyMultiplier = 0.5f;

    private bool groundedPlayer = true;
    private bool isCrouching = false;
    private float bobTimer;
    private Vector3 baseCamPos;
    private Vector3 currentCameraOffset;

    public bool OnGround => groundedPlayer;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        groundCheck = GetComponentInChildren<GroundCheck>();

        if (cameraTarget != null)
        {
            cameraTarget.localPosition = standCameraOffset;
            baseCamPos = standCameraOffset;
            currentCameraOffset = standCameraOffset;
        }
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded || groundCheck.OnGround();

        HandleCrouchInput();
        HandleMovement();
        HandleGravity();
        UpdateCrouchHeightSmooth();
        HandleHeadBobAndSway();
    }

    private void HandleMovement()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        float currentSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
            currentSpeed = sprintSpeed;
        else if (isCrouching)
            currentSpeed = crouchSpeed;

        Vector3 move = Camera.main.transform.forward * inputZ + Camera.main.transform.right * inputX;
        move.y = 0f;
        move.Normalize();

        Vector3 horizontalVelocity = move * currentSpeed;

        controller.Move(horizontalVelocity * Time.deltaTime);

        // Jump
        if (groundedPlayer && Input.GetButtonDown("Jump") && !isCrouching)
        {
            yVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravityValue);
        }
    }

    private void HandleGravity()
    {
        if (groundedPlayer && yVelocity.y < 0f)
        {
            yVelocity.y = -2f; // keeps player "stuck" to ground
        }

        // apply enhanced gravity logic
        if (yVelocity.y < 0)
        {
            // falling
            yVelocity.y += gravityValue * fallMultiplier * Time.deltaTime;
        }
        else if (yVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            // jump released early -> faster fall
            yVelocity.y += gravityValue * lowJumpMultiplier * Time.deltaTime;
        }
        else
        {
            // regular gravity
            yVelocity.y += gravityValue * Time.deltaTime;
        }

        controller.Move(yVelocity * Time.deltaTime);
    }

    private void HandleCrouchInput()
    {
        if (holdToCrouch)
            isCrouching = Input.GetKey(KeyCode.C);
        else if (Input.GetKeyDown(KeyCode.C))
            isCrouching = !isCrouching;
    }

    private void UpdateCrouchHeightSmooth()
    {
        float targetHeight = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchTransitionSpeed);
        controller.center = new Vector3(0, controller.height / 2f, 0);

        Vector3 targetOffset = isCrouching ? crouchCameraOffset : standCameraOffset;
        currentCameraOffset = Vector3.Lerp(currentCameraOffset, targetOffset, Time.deltaTime * crouchTransitionSpeed);
        baseCamPos = currentCameraOffset;
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

        float amplitude = isCrouching ? crouchBobAmplitude :
                          Input.GetKey(KeyCode.LeftShift) ? sprintBobAmplitude : walkBobAmplitude;

        float frequency = isCrouching ? crouchBobFrequency :
                          Input.GetKey(KeyCode.LeftShift) ? sprintBobFrequency : walkBobFrequency;

        float swayAmp = isCrouching ? crouchSwayAmplitude :
                        Input.GetKey(KeyCode.LeftShift) ? sprintSwayAmplitude : walkSwayAmplitude;

        bobTimer += Time.deltaTime * frequency;

        float offsetY = Mathf.Sin(bobTimer) * amplitude;
        float offsetX = Mathf.Sin(bobTimer * swayFrequencyMultiplier + Mathf.PI / 2f) * swayAmp;

        cameraTarget.localPosition = baseCamPos + new Vector3(offsetX, offsetY, 0);
    }
}
