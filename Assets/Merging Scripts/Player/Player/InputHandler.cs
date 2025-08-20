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
    public event Action onSumbit;
    public event Action onPresentEvidence;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            onJump?.Invoke();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            //Debug.Log("Submit Sent");
            onSumbit?.Invoke();
        }
    }

    public void OnPresentEvidence(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            onPresentEvidence?.Invoke();
        }
    }
}
