using UnityEngine;
using Unity.Cinemachine;

public class CamerTarget : MonoBehaviour
{
    InputHandler controls;
    Interactor interactor;

    [SerializeField] Vector2 mouseSens = new Vector2(0.09f, 0.09f);

    float xRotation, yRotation;
    Transform player;

    private Vector3 defaultLocalPos;

    private void Awake()
    {
        controls = GetComponentInParent<InputHandler>();
        interactor = GetComponentInChildren<Interactor>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player = transform.parent;

        defaultLocalPos = transform.localPosition;
    }

    private void LateUpdate()
    {
        MouseLook();
    }

    private void MouseLook()
    {
        if (GameManager.instance.CurrentState != InteractionState.None) 
            return;

        Vector2 mouseDelta = controls.lookInput;

        xRotation -= mouseDelta.y * mouseSens.x;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        yRotation += mouseDelta.x * mouseSens.y;
        player.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    public void ResetCameraPivot()
    {
        transform.localPosition = defaultLocalPos;
    }
}
