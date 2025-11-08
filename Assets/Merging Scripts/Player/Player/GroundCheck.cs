using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check Settings")]
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug Info (Read Only)")]
    [SerializeField] private bool isGroundedInspector = false; // <-- shows in inspector

    public bool OnGround() => isGroundedInspector;

    private void Update()
    {
        isGroundedInspector = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGroundedInspector ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
}
