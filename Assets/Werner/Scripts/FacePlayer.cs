using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform playerCamera;

    [Header("Face Player")]
    public Vector3 rotationOffset = new Vector3(0, 90, 0);

    void Start()
    {
        if (Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    void Update()
    {
        if (playerCamera == null) return;

        transform.LookAt(playerCamera);

        transform.Rotate(rotationOffset);
    }
}
