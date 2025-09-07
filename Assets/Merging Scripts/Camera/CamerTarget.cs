using UnityEngine;
using Unity.Cinemachine;

public class CamerTarget : MonoBehaviour
{
    // We need a player controls asset to get the mouse delta
    InputHandler controls;

    // We need a Vector2 for the mouse sensitivity amd 2 floats for the x and y rotation. [1]
    [SerializeField] Vector2 mouseSens = new Vector2(0.09f, 0.09f); // 0.09 is responsive but not snappy
    float xRotation, yRotation;

    // We need the player's transform so we can do horizontal rotation
    Transform player;

    // Initialize the player controls and enable and disable them in OnEnable and OnDisable
    private void Awake()
    {
        controls = GetComponentInParent<InputHandler>();
    }

    // In start, hide the mouse cursor and get the player's transform from the transform's parent
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player = transform.parent; // GetComponentInParent<Transform>() doesn't work.
    }

    private void LateUpdate()
    {
        MouseLook();
    }


    private void MouseLook()
    {
        if (DialogueManager.Instance.dialogueStarted) return;
        // Get the mouse delta from the look input
        Vector2 mouseDelta = controls.lookInput;

        // Up-Down:
        // Subtract the vertical mouse delta from xRotation and clamp xRotation between -90f and 90ff [2]
        xRotation -= mouseDelta.y * mouseSens.x;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Set the transform's local rotation equal to a euler where x is xRotation.[3]
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Left-Right:
        // Add the horizontal mouse delta to the yRotation 
        yRotation += mouseDelta.x * mouseSens.y;

        // Set the player's global rotation equal to a euler where y is yRotation. [4]
        player.rotation = Quaternion.Euler(0f, yRotation, 0f);

    }
}
