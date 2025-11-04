using UnityEngine;

public class EnableOnClueInteract : EvidenceObject
{
    [Header("Objects to Enable (Optional)")]
    [SerializeField] private GameObject[] objectsToEnable;

    [Header("Unlock Rift Travel")]
    [SerializeField] private RiftTravel riftTravelScript; // ✅ Drag your Player object here in Inspector

    public override void Interact()
    {
        // ✅ Collect the clue and hide item if necessary
        base.Interact();

        // ✅ Enable any objects you want to appear after clue pickup
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"Enabled object: {obj.name}");
            }
        }

        // ✅ Unlock Rift Travel system (player can now use portal)
        if (riftTravelScript != null)
        {
            riftTravelScript.UnlockPortals();
            Debug.Log("✅ Rift portals unlocked from clue interaction!");
        }
        else
        {
            Debug.LogWarning("⚠️ No RiftTravel reference assigned on EnableOnClueInteract!");
        }
    }
}
