using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance { get; private set; }

    private AudioSource audioSource;
    private float targetVolume = 0.01f;
    private float fadeSpeed = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            return;

        audioSource.loop = true;
        audioSource.volume = targetVolume;
        audioSource.Play();
    }

    private void Update()
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.MoveTowards(
                audioSource.volume,
                targetVolume,
                Time.unscaledDeltaTime * fadeSpeed
            );
        }
    }

    public void MuteMusic(bool mute)
    {
        targetVolume = mute ? 0f : 0.01f;
    }

    public void SetVolume(float volume)
    {
        targetVolume = Mathf.Clamp01(volume);
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOutAndStop());
    }

    private System.Collections.IEnumerator FadeOutAndStop()
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0f)
        {
            audioSource.volume -= Time.unscaledDeltaTime * fadeSpeed;
            yield return null;
        }

        audioSource.Stop();
    }
}
