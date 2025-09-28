using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;


public class TimeTravel : MonoBehaviour
{
    
    [Header("Teleport Settings")]
    [SerializeField] private float mapOffsetZ = 200f;
    [SerializeField] private float delayBeforeTeleport = 2f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode teleportKey = KeyCode.Q;

    [Header("Video Settings")]
    [SerializeField] private VideoClip enterRiftVideo;
    [SerializeField] private VideoClip exitRiftVideo;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private GameObject videoCanvas;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Portal Suction Effect")]
    [SerializeField] private float suctionDuration = 2f;
    [SerializeField] private float maxSuctionIntensity = 0.3f;
    [SerializeField] private AnimationCurve suctionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Particle Effects (on the player)")]
    public ParticleSystem effect1;
    public ParticleSystem effect2;
    public ParticleSystem effect3;

    [Header("Crystals")]
    [SerializeField] private List<MeshRenderer> map1Crystals;
    [SerializeField] private List<MeshRenderer> map2Crystals;
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private float triggerDistance = 5f;

    [Header("UI Popup")]
    [SerializeField] private GameObject teleportPopup;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    [SerializeField] private Text popupLabel;

    [Header("UI Prompt")]
    [SerializeField] private Text interactLabel;

    private PlayerMovement cachedMovement;
    private CharacterController controller;
    private CamerTarget cameraLook;
    private VideoPlayer videoPlayer;
    private CanvasGroup videoCanvasGroup;
    private RiftCrystalReturn returnScript;

    private Dictionary<MeshRenderer, Material[]> originalMats = new Dictionary<MeshRenderer, Material[]>();
    private MeshRenderer activeCrystal = null;
    private bool isTeleporting = false;
    private bool popupActive = false;
    private bool pendingFromMap1;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;

    void Awake()
    {
        cachedMovement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        cameraLook = GetComponentInChildren<CamerTarget>();

        returnScript = FindFirstObjectByType<RiftCrystalReturn>();

        foreach (var c in map1Crystals)
            if (c != null) originalMats[c] = c.materials;
        foreach (var c in map2Crystals)
            if (c != null) originalMats[c] = c.materials;

        if (teleportPopup != null) teleportPopup.SetActive(false);
        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoClicked);

        if (interactLabel != null) interactLabel.gameObject.SetActive(false);

        InitializeVideoSystem();
    }

    void InitializeVideoSystem()
    {
        if (videoCanvas != null)
        {
            videoCanvasGroup = videoCanvas.GetComponent<CanvasGroup>();
            if (videoCanvasGroup == null)
                videoCanvasGroup = videoCanvas.AddComponent<CanvasGroup>();

            videoCanvasGroup.alpha = 0f;
            videoCanvas.SetActive(false);

            // Make sure canvas is set to full screen overlay
            Canvas canvas = videoCanvas.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 9999;
            }
        }

        if (videoDisplay != null)
        {
            // Make sure the RawImage covers the entire screen
            RectTransform rect = videoDisplay.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            // Get or add VideoPlayer component
            videoPlayer = videoDisplay.GetComponent<VideoPlayer>();
            if (videoPlayer == null)
                videoPlayer = videoDisplay.gameObject.AddComponent<VideoPlayer>();

            // Use RenderTexture mode for fullscreen control
            videoPlayer.playOnAwake = false;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.skipOnDrop = false;

            // Create and assign a RenderTexture for fullscreen
            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;
            videoDisplay.texture = renderTexture;

            // Set audio output
            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

            // Subscribe to events
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.errorReceived += OnVideoError;
            videoPlayer.prepareCompleted += OnVideoPrepared;

            Debug.Log("VideoPlayer initialized with RenderTexture for fullscreen");
        }
        else
        {
            Debug.LogError("VideoDisplay RawImage is not assigned!");
        }
    }

    void Update()
    {
        if (popupActive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnNoClicked();
            }
            return;
        }

        MeshRenderer nearest = GetNearestCrystal();
        bool inRange = nearest != null;

        if (inRange && activeCrystal != nearest)
        {
            ClearHighlight();
            HighlightCrystal(nearest);
            ShowInteractPrompt(true);
        }
        else if (!inRange && activeCrystal != null)
        {
            ClearHighlight();
            ShowInteractPrompt(false);
        }

        if (inRange && !isTeleporting && Input.GetKeyDown(teleportKey))
        {
            pendingFromMap1 = map1Crystals.Contains(nearest);
            ShowPopup();
            ShowInteractPrompt(false);
        }
    }

    MeshRenderer GetNearestCrystal()
    {
        MeshRenderer nearest = null;
        float minDist = Mathf.Infinity;

        foreach (var c in map1Crystals)
        {
            if (c == null) continue;
            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < triggerDistance && dist < minDist)
            {
                nearest = c;
                minDist = dist;
            }
        }

        foreach (var c in map2Crystals)
        {
            if (c == null) continue;
            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < triggerDistance && dist < minDist)
            {
                nearest = c;
                minDist = dist;
            }
        }

        return nearest;
    }

    void ShowInteractPrompt(bool state)
    {
        if (interactLabel != null)
        {
            interactLabel.text = "Press Q";
            interactLabel.gameObject.SetActive(state);
        }
    }

    void ShowPopup()
    {
        popupActive = true;
        if (teleportPopup != null) teleportPopup.SetActive(true);

        if (cachedMovement != null) cachedMovement.enabled = false;
        if (cameraLook != null) cameraLook.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (popupLabel != null)
        {
            popupLabel.text = pendingFromMap1 ? "Look into the Rift?" : "Exit the Rift?";
        }
    }

    void HidePopup()
    {
        popupActive = false;
        if (teleportPopup != null) teleportPopup.SetActive(false);

        if (cachedMovement != null) cachedMovement.enabled = true;
        if (cameraLook != null) cameraLook.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnYesClicked()
    {
        HidePopup();
        StartCoroutine(TeleportSequence(pendingFromMap1));
    }

    void OnNoClicked()
    {
        HidePopup();
    }

    IEnumerator TeleportSequence(bool fromMap1)
    {
        isTeleporting = true;

        // Disable player control
        if (cachedMovement != null) cachedMovement.enabled = false;
        if (cameraLook != null) cameraLook.enabled = false;

        VideoClip targetClip = fromMap1 ? enterRiftVideo : exitRiftVideo;

        if (videoPlayer != null && targetClip != null)
        {
            Debug.Log($"Starting teleport sequence: {(fromMap1 ? "Entering" : "Exiting")} rift");

            if (fromMap1)
            {
                // ENTERING RIFT: Play suction effect before video
                yield return StartCoroutine(PlayPortalSuctionEffect());
            }

            yield return StartCoroutine(PlayVideoFullDuration(targetClip));
        }
        else
        {
            Debug.Log("Video not available, using fallback particles");
            // Fallback to particle effects
            PlayEffects();
            yield return new WaitForSeconds(delayBeforeTeleport);
            StopEffects();
            DeactivateEffects();
        }

        Debug.Log("Video finished, performing teleport");

        // Perform teleport
        Vector3 pos = transform.position;
        pos = fromMap1 ? new Vector3(pos.x, pos.y, pos.z + mapOffsetZ)
                       : new Vector3(pos.x, pos.y, pos.z - mapOffsetZ);

        if (controller != null)
        {
            controller.enabled = false;
            transform.position = pos;
            controller.enabled = true;
        }
        else
        {
            transform.position = pos;
        }

        // Re-enable player control
        if (cachedMovement != null) cachedMovement.enabled = true;
        if (cameraLook != null) cameraLook.enabled = true;

        // Handle rift return logic
        if (returnScript != null)
        {
            returnScript.SuppressReturn(2f);

            if (!fromMap1)
            {
                returnScript.ForceExitRift();
            }
        }

        isTeleporting = false;
        Debug.Log("Teleport sequence completed");
    }

    IEnumerator PlayPortalSuctionEffect()
    {
        Debug.Log("Starting portal suction effect");

        // Store original camera position
        if (cameraLook != null)
        {
            originalCameraPosition = cameraLook.transform.localPosition;
            originalCameraRotation = cameraLook.transform.localRotation;
        }

        // Start particle effects
        PlayEffects();

        float elapsed = 0f;

        while (elapsed < suctionDuration)
        {
            float progress = elapsed / suctionDuration;
            float intensity = suctionCurve.Evaluate(progress) * maxSuctionIntensity;

            // Apply camera shake/wobble to simulate suction
            if (cameraLook != null)
            {
                // Add random wobble to camera
                Vector3 shake = new Vector3(
                    Random.Range(-intensity, intensity),
                    Random.Range(-intensity, intensity),
                    Random.Range(-intensity * 0.5f, intensity * 0.5f)
                );

                cameraLook.transform.localPosition = originalCameraPosition + shake;

                // Slight rotation wobble
                Vector3 rotationShake = new Vector3(
                    Random.Range(-intensity * 10f, intensity * 10f),
                    Random.Range(-intensity * 10f, intensity * 10f),
                    Random.Range(-intensity * 5f, intensity * 5f)
                );
                cameraLook.transform.localRotation = Quaternion.Euler(rotationShake) * originalCameraRotation;
            }

            // Increase particle effects intensity
            if (effect1 != null) effect1.transform.localScale = Vector3.one * (1f + intensity * 2f);
            if (effect2 != null) effect2.transform.localScale = Vector3.one * (1f + intensity * 2f);
            if (effect3 != null) effect3.transform.localScale = Vector3.one * (1f + intensity * 2f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Portal suction effect completed");
    }

    IEnumerator PlayVideoFullDuration(VideoClip clip)
    {
        Debug.Log($"Preparing to play video: {clip.name}");

        // Reset camera after suction effect
        if (cameraLook != null && pendingFromMap1)
        {
            cameraLook.transform.localPosition = originalCameraPosition;
            cameraLook.transform.localRotation = originalCameraRotation;
        }

        // Set up video
        videoPlayer.clip = clip;

        // Show canvas first
        videoCanvas.SetActive(true);

        // Fade in
        yield return StartCoroutine(FadeVideoIn());

        // Show cursor during video
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Starting video playback");

        // Play video
        videoPlayer.Play();

        // Wait for the FULL video length
        float videoLength = (float)clip.length;
        Debug.Log($"Waiting for full video duration: {videoLength} seconds");

        yield return new WaitForSeconds(videoLength);

        Debug.Log("Video duration completed, starting fade out");

        // Fade out
        yield return StartCoroutine(FadeVideoOut());

        // Stop particle effects
        StopEffects();
        DeactivateEffects();

        // Hide cursor after video
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("Video playback fully completed");
    }

    IEnumerator FadeVideoIn()
    {
        if (videoCanvasGroup == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            videoCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        videoCanvasGroup.alpha = 1f;
    }

    IEnumerator FadeVideoOut()
    {
        if (videoCanvasGroup == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            videoCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        videoCanvasGroup.alpha = 0f;

        videoCanvas.SetActive(false);
        if (videoPlayer != null)
            videoPlayer.Stop();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Video finished event received");
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Video prepared successfully");
    }

    void OnVideoError(VideoPlayer vp, string message)
    {
        Debug.LogError($"Video Player Error: {message}");

        // Fallback to particles if video fails
        if (isTeleporting)
        {
            StartCoroutine(VideoFallback());
        }
    }

    IEnumerator VideoFallback()
    {
        Debug.Log("Using fallback particle effects due to video error");

        if (videoCanvas != null)
        {
            videoCanvas.SetActive(false);
        }

        PlayEffects();
        yield return new WaitForSeconds(delayBeforeTeleport);
        StopEffects();
        DeactivateEffects();

        // Continue with teleport
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // --- Highlight helpers ---
    void HighlightCrystal(MeshRenderer crystal)
    {
        if (crystal == null) return;

        Material[] newMats = new Material[originalMats[crystal].Length + 1];
        for (int i = 0; i < originalMats[crystal].Length; i++)
            newMats[i] = originalMats[crystal][i];
        newMats[newMats.Length - 1] = outlineMaterial;
        crystal.materials = newMats;
        activeCrystal = crystal;
    }

    void ClearHighlight()
    {
        if (activeCrystal == null) return;
        activeCrystal.materials = originalMats[activeCrystal];
        activeCrystal = null;
    }

    // --- Particle helpers ---
    void PlayEffects()
    {
        if (effect1) { effect1.gameObject.SetActive(true); effect1.Play(true); }
        if (effect2) { effect2.gameObject.SetActive(true); effect2.Play(true); }
        if (effect3) { effect3.gameObject.SetActive(true); effect3.Play(true); }
    }

    void StopEffects()
    {
        if (effect1) effect1.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (effect2) effect2.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        if (effect3) effect3.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    void DeactivateEffects()
    {
        if (effect1) effect1.gameObject.SetActive(false);
        if (effect2) effect2.gameObject.SetActive(false);
        if (effect3) effect3.gameObject.SetActive(false);
    }
}
