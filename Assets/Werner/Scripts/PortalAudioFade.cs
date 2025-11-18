using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PortalAudioFade : MonoBehaviour
{
    [SerializeField] private Transform player; 
    [SerializeField] private float maxDistance = 10;
    [SerializeField] private float fadeStart = 6;

    private AudioSource portalAudio;

    void Start()
    {
        portalAudio = GetComponent<AudioSource>();
        portalAudio.spatialBlend = 1f;
        portalAudio.loop = true;
        if (!portalAudio.isPlaying) portalAudio.Play();
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.position);

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
            portalAudio.volume = 0.3f;
        }
    }
}
