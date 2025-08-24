using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mainMenuPanel;
    public Button startButton;
    public Button quitButton;

    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public string videoFileName = "IntroVideo.mp4"; // change file name here
    public RawImage videoDisplay;
    public AudioSource videoAudio;

    [Header("Scene Management")]
    public string gameSceneName = "GameScene";//change scene name here
    public GameObject loadingPanel;

    void Start()
    {
        
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);

        
        StartCoroutine(SetupVideoPlayer());
    }

    IEnumerator SetupVideoPlayer()
    {
        
        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, videoAudio);

        
        videoPlayer.Play();
        videoAudio.Play();
    }

    public void StartGame()
    {
        
        videoPlayer.Stop();
        videoAudio.Stop();

        if (loadingPanel != null)
            loadingPanel.SetActive(true);

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void Update()
    {
        
        if (Input.anyKeyDown && !Input.GetMouseButton(0))
        {
            StartGame();
        }

        
        if (!videoPlayer.isPlaying && videoPlayer.isPrepared)
        {
            videoPlayer.Play();
        }
    }
}
