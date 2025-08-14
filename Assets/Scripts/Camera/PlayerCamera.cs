using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform player;


    private void LateUpdate()
    {
        if (!player) return;

        transform.position = player.position;
    }
}
