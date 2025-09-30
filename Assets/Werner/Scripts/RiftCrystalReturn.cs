using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.UI;

public class RiftCrystalReturn : MonoBehaviour
{
    [Header("Rift Settings")]
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float delayBeforeReturn = 1.5f;
    [SerializeField] private float mapOffsetZ = 200f;

    [Header("Player Reference")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraHolder;

    [Header("Shake Settings")]
    [SerializeField] private float shakeStartDistance = 5f;
    [SerializeField] private float maxShakeStrength = 0.3f;
    [SerializeField] private float shakeSpeed = 20f;

    [Header("Video Settings")]
    [SerializeField] private VideoClip returnRiftVideo;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private GameObject videoCanvas;
    [SerializeField] private float fadeDuration = 1f;

    private CharacterController controller;
    private PlayerMovement movement;
    private bool isReturning = false;
    private bool playerInRift = false;
    private bool isTeleportingBack = false;

    private Vector3 camOriginalLocalPos;

    private VideoPlayer videoPlayer;
    private CanvasGroup videoCanvasGroup;

    void Start()
    {
        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null) player = obj.transform;
        }

        if (player != null)
        {
            controller = player.GetComponent<CharacterController>();
            movement = player.GetComponent<PlayerMovement>();
        }

        if (cameraHolder != null)
        {
            camOriginalLocalPos = cameraHolder.localPosition;
        }

        InitializeVideoSystem();
    }

    void LateUpdate()
    {
        if (player == null || isReturning) return;

        if (!playerInRift && player.position.z > 100f)
        {
            playerInRift = true;
        }

        if (playerInRift)
        {
            float dist = Vector3.Distance(player.position, transform.position);

            float intensity = 0f;
            if (dist > shakeStartDistance)
            {
                float t = (dist - shakeStartDistance) / (maxDistance - shakeStartDistance);
                intensity = Mathf.Clamp01(t);
            }

            if (isTeleportingBack)
            {
                ApplyCameraShake(1f); 
            }
            else
            {
                ApplyCameraShake(intensity);
            }

            if (dist > maxDistance && !isTeleportingBack)
            {
                Debug.Log($"Player left rift bubble! Distance = {dist}");
                StartCoroutine(ReturnToMap1());
            }
        }
    }

    IEnumerator ReturnToMap1()
    {
        isReturning = true;
        isTeleportingBack = true;
        if (movement != null) movement.enabled = false;

        // Instead of particles, play video
        if (videoPlayer != null && returnRiftVideo != null)
        {
            yield return StartCoroutine(PlayVideoFullDuration(returnRiftVideo));
        }
        else
        {
            // fallback if no video
            yield return new WaitForSeconds(delayBeforeReturn);
        }

        Vector3 pos = player.position;
        pos = new Vector3(pos.x, pos.y, pos.z - mapOffsetZ);

        if (controller != null)
        {
            controller.enabled = false;
            player.position = pos;
            controller.enabled = true;
        }
        else
        {
            player.position = pos;
        }

        if (movement != null) movement.enabled = true;

        if (cameraHolder != null)
            cameraHolder.localPosition = camOriginalLocalPos;

        Debug.Log("Player teleported back to Map 1!");

        playerInRift = false;
        isReturning = false;
        isTeleportingBack = false;
    }

    // --- NEW: suppress auto-return after manual teleport ---
    public void SuppressReturn(float duration = 2f)
    {
        StartCoroutine(SuppressReturnRoutine(duration));
    }

    private IEnumerator SuppressReturnRoutine(float duration)
    {
        isReturning = true;
        yield return new WaitForSeconds(duration);
        isReturning = false;
    }

    public void ForceExitRift()
    {
        playerInRift = false;
        isReturning = false;
        isTeleportingBack = false;
    }

    // ---------------- VIDEO HELPERS ----------------
    void InitializeVideoSystem()
    {
        if (videoCanvas != null)
        {
            videoCanvasGroup = videoCanvas.GetComponent<CanvasGroup>();
            if (videoCanvasGroup == null)
                videoCanvasGroup = videoCanvas.AddComponent<CanvasGroup>();

            videoCanvasGroup.alpha = 0f;
            videoCanvas.SetActive(false);

            Canvas canvas = videoCanvas.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 9999;
            }
        }

        if (videoDisplay != null)
        {
            RectTransform rect = videoDisplay.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            videoPlayer = videoDisplay.GetComponent<VideoPlayer>();
            if (videoPlayer == null)
                videoPlayer = videoDisplay.gameObject.AddComponent<VideoPlayer>();

            videoPlayer.playOnAwake = false;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.skipOnDrop = false;

            RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;
            videoDisplay.texture = renderTexture;

            videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        }
        else
        {
            Debug.LogError("VideoDisplay RawImage is not assigned!");
        }
    }

    IEnumerator PlayVideoFullDuration(VideoClip clip)
    {
        videoPlayer.clip = clip;
        videoCanvas.SetActive(true);

        yield return StartCoroutine(FadeVideoIn());

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        videoPlayer.Play();
        float videoLength = (float)clip.length;
        yield return new WaitForSeconds(videoLength);

        yield return StartCoroutine(FadeVideoOut());

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

    void ApplyCameraShake(float intensity)
    {
        if (cameraHolder == null) return;

        float scaledIntensity = Mathf.Pow(intensity, 2f);
        float shakeAmount = maxShakeStrength * scaledIntensity;

        Vector3 offset = new Vector3(
            (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f,
            (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f,
            0f
        ) * shakeAmount;

        cameraHolder.localPosition = camOriginalLocalPos + offset;
    }
}
