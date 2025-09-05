using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private Transform playerCamera;

    [Header("Adjust this if your object faces the wrong way")]
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

        // Apply extra rotation to fix the facing direction
        transform.Rotate(rotationOffset);
    }
}
