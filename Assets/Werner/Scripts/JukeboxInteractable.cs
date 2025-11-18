using UnityEngine;
using UnityEngine.UI;

public class JukeboxInteractable : Interactable
{
    [Header("UI Elements")]
    [SerializeField] private GameObject jukeboxUI;
    [SerializeField] private Button song1Button;
    [SerializeField] private Button song2Button;
    [SerializeField] private Button song3Button;
    [SerializeField] private Button song4Button;
    [SerializeField] private Button song5Button;
    [SerializeField] private Button song6Button;
    [SerializeField] private Button song7Button;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button closeButton;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource song1;
    [SerializeField] private AudioSource song2;
    [SerializeField] private AudioSource song3;
    [SerializeField] private AudioSource song4;
    [SerializeField] private AudioSource song5;
    [SerializeField] private AudioSource song6;
    [SerializeField] private AudioSource song7;

    [Header("Highlighting")]
    [SerializeField] private HighlightTarget highlightTarget;

    private bool isPlayerNearby = false;
    private AudioSource currentSong;
    private BackgroundMusicManager bgm;

    private void Start()
    {
        if (jukeboxUI != null)
            jukeboxUI.SetActive(false);

        bgm = FindObjectOfType<BackgroundMusicManager>();

        if (song1Button != null) song1Button.onClick.AddListener(() => PlaySong(song1));
        if (song2Button != null) song2Button.onClick.AddListener(() => PlaySong(song2));
        if (song3Button != null) song3Button.onClick.AddListener(() => PlaySong(song3));
        if (song4Button != null) song4Button.onClick.AddListener(() => PlaySong(song4));
        if (song5Button != null) song5Button.onClick.AddListener(() => PlaySong(song5));
        if (song6Button != null) song6Button.onClick.AddListener(() => PlaySong(song6));
        if (song7Button != null) song7Button.onClick.AddListener(() => PlaySong(song7));
        if (stopButton != null) stopButton.onClick.AddListener(StopAllSongs);
        if (closeButton != null) closeButton.onClick.AddListener(CloseUI);
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (currentSong != null && !currentSong.isPlaying)
        {
            currentSong = null;
            ResumeBackgroundMusic();
        }
    }

    public override void Interact()
    {
        base.Interact();
        OpenUI();
    }

    private void PlaySong(AudioSource song)
    {
        StopAllSongs();

        if (song != null)
        {
            currentSong = song;
            MuteBackgroundMusic();
            song.Play();
        }
    }

    private void StopAllSongs()
    {
        if (song1 != null) song1.Stop();
        if (song2 != null) song2.Stop();
        if (song3 != null) song3.Stop();
        if (song4 != null) song4.Stop();
        if (song5 != null) song5.Stop();
        if (song6 != null) song6.Stop();
        if (song7 != null) song7.Stop();

        currentSong = null;
        ResumeBackgroundMusic();
    }

    private void OpenUI()
    {
        if (jukeboxUI == null) return;

        jukeboxUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    private void CloseUI()
    {
        if (jukeboxUI == null) return;

        jukeboxUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    private void MuteBackgroundMusic()
    {
        if (bgm != null)
            bgm.MuteMusic(true);
    }

    private void ResumeBackgroundMusic()
    {
        if (bgm != null)
            bgm.MuteMusic(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (highlightTarget != null)
                highlightTarget.HighlightObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (highlightTarget != null)
                highlightTarget.ClearHighlight();
        }
    }
}
