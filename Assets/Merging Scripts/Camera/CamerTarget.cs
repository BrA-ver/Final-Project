using UnityEngine;
using Unity.Cinemachine;

public class CamerTarget : MonoBehaviour
{
    [SerializeField] Vector2 mouseSens = new Vector2(0.09f, 0.09f);
    [SerializeField] Vector2 limitAngle = new Vector2(-30f, 70f);
    float xRotation, yRotation;

    Transform player;
    Vector3 offset;
    Vector2 LookInput = Vector2.zero;

    InputHandler input;
    // In start, hide the mouse cursor and get the player's transform from the transform's parent
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player = transform.parent;
        offset = transform.position - player.position;
        transform.SetParent(null);

        CinemachineTargetGroup targetGroup = FindObjectOfType<CinemachineTargetGroup>();
        Debug.Log(targetGroup == null);
        targetGroup.AddMember(transform, 10f, 1f);

        input = FindObjectOfType<InputHandler>();
    }


    private void LateUpdate()
    {
        transform.position = player.transform.position + offset;
        MouseLook();
    }


    private void MouseLook()
    {
        LookInput = input.lookInput;

        xRotation -= LookInput.y * mouseSens.x;
        xRotation = Mathf.Clamp(xRotation, limitAngle.x, limitAngle.y);

        yRotation += LookInput.x * mouseSens.y;

        // Set the transform's local rotation equal to a euler where x is xRotation.[3]
        transform.rotation = Quaternion.Euler(xRotation, yRotation, transform.eulerAngles.z);
    }
}
