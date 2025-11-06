using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingZoneSwitcher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform realityProcessing;
    [SerializeField] private Transform riftProcessing;
    [SerializeField] private Volume globalVolume1;  // Reality Post-Processing
    [SerializeField] private Volume globalVolume2;  // Rift Post-Processing

    [Header("Settings")]
    [SerializeField] private float switchRadius = 100f;

    [Header("Objects To Disable / Enable")]
    [SerializeField] private GameObject[] objectsToDisable; // Portals or other objects

    // ✅ Only enable this logic after clue is collected
    private bool clueCollected = false;

    // ✅ Called from EnableOnClueInteract
    public void SetClueCollected(bool value)
    {
        clueCollected = value;
        Debug.Log("✅ Clue collected — PostProcessing + Portal logic now active.");
    }

    void Update()
    {
        if (!clueCollected || player == null) return;

        float distToReality = Vector3.Distance(player.position, realityProcessing.position);
        float distToRift = Vector3.Distance(player.position, riftProcessing.position);

        bool isInReality = distToReality <= switchRadius;
        bool isInRift = distToRift <= switchRadius;

        // ✅ Player in Reality Zone (Map 1) → Disable portals
        if (isInReality)
        {
            globalVolume1.enabled = true;   // Enable Reality Post-Processing
            globalVolume2.enabled = false;

            SetObjectsActive(false);        // Disable portals/objects
            return;
        }

        // ✅ Player in Rift Zone (Map 2) → Enable portals
        if (isInRift)
        {
            globalVolume1.enabled = false;
            globalVolume2.enabled = true;

            SetObjectsActive(true);         // Enable portals
            return;
        }

        // ✅ Outside both → Turn off effects, keep objects as they were
        globalVolume1.enabled = false;
        globalVolume2.enabled = false;
    }

    // ✅ Helper: enable/disable portals or objects
    private void SetObjectsActive(bool state)
    {
        if (objectsToDisable == null) return;

        foreach (var obj in objectsToDisable)
        {
            if (obj != null) obj.SetActive(state);
        }
    }
}
