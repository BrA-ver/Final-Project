using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform player;


    private void LateUpdate()
    {
        if (!player)
        {
            FindPlayer();
            return;
        }

        transform.position = player.position;
    }

    void FindPlayer()
    {
        player = FindObjectOfType<Player>().transform;
    }
}
