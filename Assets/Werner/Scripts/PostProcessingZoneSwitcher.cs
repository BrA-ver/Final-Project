using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingZoneSwitcher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform realityProcessing;   // Empty GameObject
    [SerializeField] private Transform riftProcessing;      // Empty GameObject
    [SerializeField] private Volume globalVolume1;          // Reality Volume
    [SerializeField] private Volume globalVolume2;          // Rift Volume

    [Header("Settings")]
    [SerializeField] private float switchRadius = 100f;

    void Update()
    {
        if (player == null || realityProcessing == null || riftProcessing == null) return;

        float distToReality = Vector3.Distance(player.position, realityProcessing.position);
        float distToRift = Vector3.Distance(player.position, riftProcessing.position);

        if (distToReality <= switchRadius && distToReality < distToRift)
        {
            // Player is in Reality zone
            globalVolume1.enabled = true;
            globalVolume2.enabled = false;
        }
        else if (distToRift <= switchRadius && distToRift < distToReality)
        {
            // Player is in Rift zone
            globalVolume1.enabled = false;
            globalVolume2.enabled = true;
        }
        else
        {
            // Player is outside both zones → disable both or keep last state
            globalVolume1.enabled = false;
            globalVolume2.enabled = false;
        }
    }
}
