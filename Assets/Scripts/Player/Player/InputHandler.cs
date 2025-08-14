using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    public Vector2 moveInput;
    public InputActionReference jump;
    public bool jumpPressed => jump.action.triggered;

    public event Action onJump;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            onJump?.Invoke();
    }
}
