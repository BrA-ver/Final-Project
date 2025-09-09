using UnityEngine;
using System.Collections;

public class RiftCrystalReturn : MonoBehaviour
{
    [Header("Rift Settings")]
    [SerializeField] private float maxDistance = 15f;
    [SerializeField] private float delayBeforeReturn = 1.5f;
    [SerializeField] private float mapOffsetZ = 200f;

    [Header("Effects")]
    public ParticleSystem effect1;
    public ParticleSystem effect2;
    public ParticleSystem effect3;

    [Header("Player Reference")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform cameraHolder;

    [Header("Shake Settings")]
    [SerializeField] private float shakeStartDistance = 5f;
    [SerializeField] private float maxShakeStrength = 0.3f;
    [SerializeField] private float shakeSpeed = 20f;

    private CharacterController controller;
    private PlayerMovement movement;
    private bool isReturning = false;
    private bool playerInRift = false;
    private bool isTeleportingBack = false; // ✅ NEW

    private Vector3 camOriginalLocalPos;

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

            // ✅ Keep shaking while teleport countdown is active
            if (isTeleportingBack)
            {
                ApplyCameraShake(1f); // full intensity during teleport delay
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
        isTeleportingBack = true; // ✅ start shake loop
        if (movement != null) movement.enabled = false;

        PlayEffects();

        // ✅ custom wait loop that keeps shaking
        float elapsed = 0f;
        while (elapsed < delayBeforeReturn)
        {
            ApplyCameraShake(1f); // shake hard while waiting
            elapsed += Time.deltaTime;
            yield return null;
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

        StopEffects();
        DeactivateEffects();

        if (movement != null) movement.enabled = true;

        if (cameraHolder != null)
            cameraHolder.localPosition = camOriginalLocalPos;

        Debug.Log("Player teleported back to Map 1!");

        playerInRift = false;
        isReturning = false;
        isTeleportingBack = false; // ✅ stop shake after teleport
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

    // --- NEW: reset state when teleporting back to Map 1 ---
    public void ForceExitRift()
    {
        playerInRift = false;
        isReturning = false;
        isTeleportingBack = false;
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
