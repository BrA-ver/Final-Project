using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PortalAudioFade : MonoBehaviour
{
    [SerializeField] private Transform player;       // Assign your player (or camera) in Inspector
    [SerializeField] private float maxDistance = 10; // Distance where sound is fully silent
    [SerializeField] private float fadeStart = 6;    // Distance where volume begins to drop

    private AudioSource portalAudio;

    void Start()
    {
        portalAudio = GetComponent<AudioSource>();
        portalAudio.spatialBlend = 1f; // make sure it stays 3D
        portalAudio.loop = true;
        if (!portalAudio.isPlaying) portalAudio.Play();
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Custom fade: full volume nearby, fade linearly, hard mute beyond maxDistance
        if (distance > maxDistance)
        {
            portalAudio.volume = 0f;
        }
        else if (distance > fadeStart)
        {
            float t = 1f - ((distance - fadeStart) / (maxDistance - fadeStart));
            portalAudio.volume = Mathf.Clamp01(t);
        }
        else
        {
            portalAudio.volume = 1f;
        }
    }
}
