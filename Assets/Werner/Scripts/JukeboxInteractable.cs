using UnityEngine;
using UnityEngine.UI;

public class JukeboxInteractable : Interactable
{
    [Header("UI Elements")]
    [SerializeField] private GameObject jukeboxUI;
    [SerializeField] private Button song1Button;
    [SerializeField] private Button song2Button;
    [SerializeField] private Button song3Button;
    [SerializeField] private Button stopButton;     // ✅ New Stop Button
    [SerializeField] private Button closeButton;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource song1;
    [SerializeField] private AudioSource song2;
    [SerializeField] private AudioSource song3;

    [Header("Highlighting")]
    [SerializeField] private HighlightTarget highlightTarget;

    private bool isPlayerNearby = false;

    private void Start()
    {
        // Hide UI on start
        if (jukeboxUI != null)
            jukeboxUI.SetActive(false);

        // Hook up button listeners
        if (song1Button != null) song1Button.onClick.AddListener(() => PlaySong(song1));
        if (song2Button != null) song2Button.onClick.AddListener(() => PlaySong(song2));
        if (song3Button != null) song3Button.onClick.AddListener(() => PlaySong(song3));
        if (stopButton != null) stopButton.onClick.AddListener(StopAllSongs);   // ✅ Added listener
        if (closeButton != null) closeButton.onClick.AddListener(CloseUI);
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
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
            song.Play();
    }

    private void StopAllSongs()
    {
        if (song1 != null) song1.Stop();
        if (song2 != null) song2.Stop();
        if (song3 != null) song3.Stop();
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
