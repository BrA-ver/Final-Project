using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    public Vector2 moveInput;
    public Vector2 lookInput;
    public InputActionReference jump;
    public bool jumpPressed => jump.action.triggered;

    public event Action onJump;
    public event Action onSumbit;
    public event Action onPresentEvidence;
    public event Action onInteract;
    public event Action onCaseBoard;

    public InputAction jumpAction;

    public static InputHandler instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (CutsceneManager.IsCutsceneActive)
        {
            UnlockCursor();
            return;
        }

        LockCursor();
    }

    public void LockCursor()
    {
        if (CutsceneManager.IsCutsceneActive)
            return;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (CutsceneManager.IsCutsceneActive)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (CutsceneManager.IsCutsceneActive)
        {
            lookInput = Vector2.zero;
            return;
        }

        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (CutsceneManager.IsCutsceneActive) return;
        if (context.started)
            onJump?.Invoke();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.started)
            onSumbit?.Invoke();
    }

    public void OnPresentEvidence(InputAction.CallbackContext context)
    {
        if (context.started)
            onPresentEvidence?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (CutsceneManager.IsCutsceneActive) return;
        if (context.started)
            onInteract?.Invoke();
    }

    public void OnCaseBoard(InputAction.CallbackContext context)
    {
        if (CutsceneManager.IsCutsceneActive) return;
        if (context.performed)
            onCaseBoard?.Invoke();
    }
}
