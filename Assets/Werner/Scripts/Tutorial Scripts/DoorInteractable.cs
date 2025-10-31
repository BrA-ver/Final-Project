using UnityEngine;

public class DoorInteractable : Interactable
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform teleportTarget; // assign in Inspector
    [SerializeField] private HighlightTarget highlightTarget;

    private bool isPlayerNearby = false;
    private Transform playerTransform;

    public override void Interact()
    {
        base.Interact();

        // ✅ Find player if not cached
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        if (playerTransform != null && teleportTarget != null)
        {
            // ✅ Find WernerMovement anywhere on the player hierarchy
            WernerMovement movement = playerTransform.GetComponentInChildren<WernerMovement>();
            if (movement == null)
                movement = playerTransform.GetComponentInParent<WernerMovement>();

            if (movement != null)
            {
                movement.TeleportTo(teleportTarget.position, teleportTarget.rotation);
                Debug.Log($"✅ Player teleported to {teleportTarget.name} at {teleportTarget.position}");
            }
            else
            {
                Debug.LogWarning("WernerMovement not found on player or its children!");
            }
        }
        else
        {
            Debug.LogWarning("Teleport failed — missing playerTransform or teleportTarget");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerTransform = other.transform;

            if (highlightTarget != null)
                highlightTarget.HighlightObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerTransform = null;

            if (highlightTarget != null)
                highlightTarget.ClearHighlight();
        }
    }
}
