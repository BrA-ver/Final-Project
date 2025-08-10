using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        // Get input from WASD or arrow keys
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        // Create movement vector relative to world axes
        Vector3 move = new Vector3(moveX, 0f, moveZ).normalized;

        // Move the player
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
}
