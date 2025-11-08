using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingZoneSwitcher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform realityProcessing;
    [SerializeField] private Transform riftProcessing;
    [SerializeField] private Volume globalVolume1;
    [SerializeField] private Volume globalVolume2;

    [Header("Settings")]
    [SerializeField] private float switchRadius = 100f;

    [Header("Objects To Disable After Leaving Zone")]
    [SerializeField] private GameObject[] objectsToDisable;

    // ✅ State Tracking
    private bool clueCollected = false; // only after clue is picked up
    private bool playerWasInside = false; // becomes true after entering zone once

    // 🟢 This gets called by EnableOnClueInteract script
    public void SetClueCollected(bool value)
    {
        clueCollected = value;
        Debug.Log("✅ Clue collected. Zone logic now active.");
    }

    void Update()
    {
        if (!clueCollected || player == null) 
            return; // ❌ Do nothing until portals are unlocked by clue

        float distToReality = Vector3.Distance(player.position, realityProcessing.position);
        float distToRift = Vector3.Distance(player.position, riftProcessing.position);

        bool isInZone = distToReality <= switchRadius || distToRift <= switchRadius;

        // ✅ Player is inside zone
        if (isInZone)
        {
            playerWasInside = true; // Now we can detect exit later

            // Enable portals if disabled
            SetObjectsActive(true);

            // Apply correct post-processing
            if (distToReality < distToRift)
            {
                globalVolume1.enabled = true;
                globalVolume2.enabled = false;
            }
            else
            {
                globalVolume1.enabled = false;
                globalVolume2.enabled = true;
            }
        }
        // ✅ Player leaves zone after having been inside
        else if (playerWasInside)
        {
            Debug.Log("❌ Player left zone — disabling portals!");
            playerWasInside = false;

            // Turn off post-processing
            globalVolume1.enabled = false;
            globalVolume2.enabled = false;

            // Disable portals
            SetObjectsActive(false);
        }
    }

    private void SetObjectsActive(bool state)
    {
        if (objectsToDisable == null) return;

        foreach (var obj in objectsToDisable)
        {
            if (obj != null) obj.SetActive(state);
        }
    }
}
