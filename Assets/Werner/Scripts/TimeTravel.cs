using UnityEngine;
using System.Collections;

public class TimeTravel : MonoBehaviour
{
    public static TimeTravel Instance;

    [Header("Teleport Settings")]
    public float delayBeforeTeleport = 2f;
    public float teleportDistance = 100f;

    [Header("Assign particle system instances (on the player)")]
    public ParticleSystem effect1;
    public ParticleSystem effect2;
    public ParticleSystem effect3;

    [Header("(Optional) Trigger-on-tag if crystal has tag 'TimeCrystal'")]
    public bool triggerOnCrystalTag = false;
    public string crystalTag = "TimeCrystal";

    private bool isTeleporting = false;
    private bool inPresent = true;
    private PlayerMovement cachedMovement;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        cachedMovement = GetComponent<PlayerMovement>();
    }

    // If you want the PLAYER to react directly when touching a crystal (with IsTrigger = true):
    void OnTriggerEnter(Collider other)
    {
        if (!triggerOnCrystalTag || isTeleporting) return;
        if (other.CompareTag(crystalTag))
            TryStartTimeTravel();
    }

    /// <summary>Call this from the crystal to begin the sequence.</summary>
    public void TryStartTimeTravel()
    {
        if (!isTeleporting)
            StartCoroutine(TimeTravelSequence());
    }

    IEnumerator TimeTravelSequence()
    {
        isTeleporting = true;

        if (cachedMovement != null) cachedMovement.enabled = false;

        PlayEffects();

        yield return new WaitForSeconds(delayBeforeTeleport);

        Vector3 offset = new Vector3(0f, 0f, teleportDistance);
        transform.position += inPresent ? offset : -offset;
        inPresent = !inPresent;

        StopEffects();
        DeactivateEffects();

        yield return new WaitForSeconds(0.1f);

        if (cachedMovement != null) cachedMovement.enabled = true;

        isTeleporting = false;
    }

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
