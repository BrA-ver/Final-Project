using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    private static BackgroundMusicManager instance;
    private AudioSource audioSource;
    private float targetVolume = 0.01f; // default base volume
    private float fadeSpeed = 2f;      // how quickly to fade

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on BackgroundMusicManager!");
            return;
        }

        audioSource.loop = true;
        audioSource.volume = targetVolume;
        audioSource.Play();
    }

    private void Update()
    {
        // smooth fade toward targetVolume
        if (audioSource != null)
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.unscaledDeltaTime * fadeSpeed);
    }

    public void MuteMusic(bool mute)
    {
        targetVolume = mute ? 0f : 0.01f; // adjust base volume here if needed
    }

    public void SetVolume(float volume)
    {
        targetVolume = Mathf.Clamp01(volume);
    }
}
