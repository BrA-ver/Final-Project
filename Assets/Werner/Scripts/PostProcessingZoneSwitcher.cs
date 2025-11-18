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

    private bool clueCollected = false;
    private bool playerWasInside = false;

    public void SetClueCollected(bool value)
    {
        clueCollected = value;
    }

    void Update()
    {
        if (!clueCollected || player == null) 
            return;

        float distToReality = Vector3.Distance(player.position, realityProcessing.position);
        float distToRift = Vector3.Distance(player.position, riftProcessing.position);

        bool isInZone = distToReality <= switchRadius || distToRift <= switchRadius;

        if (isInZone)
        {
            playerWasInside = true;

            SetObjectsActive(true);

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
        else if (playerWasInside)
        {
            playerWasInside = false;

            globalVolume1.enabled = false;
            globalVolume2.enabled = false;

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
