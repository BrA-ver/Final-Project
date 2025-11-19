using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    public AudioMixer mixer;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        if (musicSlider != null) musicSlider.value = music;
        if (sfxSlider != null) sfxSlider.value = sfx;

        SetMusicVolume(music);
        SetSFXVolume(sfx);

        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        if (value < 0.0001f) value = 0.0001f;
        float dB = Mathf.Log10(value) * 20;
        mixer.SetFloat("Music", dB);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        if (value < 0.0001f) value = 0.0001f;
        float dB = Mathf.Log10(value) * 20;
        mixer.SetFloat("SFX", dB);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
