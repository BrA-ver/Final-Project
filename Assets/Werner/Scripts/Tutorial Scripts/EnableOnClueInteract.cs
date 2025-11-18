using UnityEngine;

public class EnableOnClueInteract : EvidenceObject
{
    [Header("Objects to Enable (Rifts / Crystals / etc.)")]
    [SerializeField] private GameObject[] objectsToEnable;

    [Header("Unlock Rift Travel for this clue")]
    [SerializeField] private RiftTravel riftTravelScript;

    public override void Interact()
    {
        base.Interact();

        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
            }
        }

        if (riftTravelScript != null)
        {
            riftTravelScript.UnlockPortals();
        }
    }
}
