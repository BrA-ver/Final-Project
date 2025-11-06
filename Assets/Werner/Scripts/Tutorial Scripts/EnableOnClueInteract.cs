using UnityEngine;

public class EnableOnClueInteract : EvidenceObject
{
    [Header("Objects to Enable (Optional)")]
    [SerializeField] private GameObject[] objectsToEnable;

    [Header("Unlock Rift Travel")]
    [SerializeField] private RiftTravel riftTravelScript;

    [Header("Post Processing Zone (Optional)")]
    [SerializeField] private PostProcessingZoneSwitcher zoneSwitcher; // ✅ Drag the zone switcher object here

    public override void Interact()
    {
        base.Interact();

        // ✅ Enable assigned objects when clue is collected
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"Enabled object: {obj.name}");
            }
        }

        // ✅ Unlock rift travel
        if (riftTravelScript != null)
        {
            riftTravelScript.UnlockPortals();
            Debug.Log("✅ Rift portals unlocked from clue interaction!");
        }
        else
        {
            Debug.LogWarning("⚠️ No RiftTravel reference assigned!");
        }

        // ✅ Tell post-process script that clue is collected
        if (zoneSwitcher != null)
        {
            zoneSwitcher.SetClueCollected(true);
        }
    }
}
