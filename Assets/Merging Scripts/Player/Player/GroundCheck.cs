using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] float checkRadius = .5f;
    [SerializeField] bool drawGizmos = false;
    [SerializeField] LayerMask groundLayer;
    public bool OnGround()
    {
        bool onGround = Physics.CheckSphere(transform.position, checkRadius, groundLayer);
        return onGround;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
