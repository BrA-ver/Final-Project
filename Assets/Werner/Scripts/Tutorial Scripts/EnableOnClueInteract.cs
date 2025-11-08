using UnityEngine;

public class EnableOnClueInteract : EvidenceObject
{
    [Header("Objects to Enable (Rifts / Crystals / etc.)")]
    [SerializeField] private GameObject[] objectsToEnable;

    [Header("Unlock Rift Travel for this clue")]
    [SerializeField] private RiftTravel riftTravelScript;

    public override void Interact()
    {
        base.Interact(); // Plays clue collection logic (sound, UI, etc.)

        // ✅ Enable assigned rifts or objects (Map1 + Map2 rifts)
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
                Debug.Log($"Enabled: {obj.name}");
            }
        }

        // ✅ Allow teleporting once clue is collected
        if (riftTravelScript != null)
        {
            riftTravelScript.UnlockPortals();
            Debug.Log("✅ Rift travel unlocked for this clue.");
        }
        else
        {
            Debug.LogWarning("⚠️ No RiftTravel script assigned!");
        }
    }
}
