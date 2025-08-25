using UnityEngine;
using System.Collections;

public class TimeTravel : MonoBehaviour
{
    public static TimeTravel Instance;

    [Header("Teleport Settings")]
    public float delayBeforeTeleport = 2f;
    public float teleportDistance = 100f;

    [Header("Assign particle system instances")]
    public ParticleSystem effect1;
    public ParticleSystem effect2;
    public ParticleSystem effect3;

    private bool isTeleporting = false;

    private bool inPresent = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isTeleporting)
        {
            StartCoroutine(TimeTravelSequence());
        }
    }

    IEnumerator TimeTravelSequence()
    {
        isTeleporting = true;

        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        PlayEffects();

        yield return new WaitForSeconds(delayBeforeTeleport);

        if (inPresent)
        {
            transform.position += new Vector3(0, 0, teleportDistance);
            inPresent = false;
        }
        else
        {
            transform.position -= new Vector3(0, 0, teleportDistance);
            inPresent = true;
        }

        StopEffects();
        DeactivateEffects();

        yield return new WaitForSeconds(0.1f);

        if (movement != null) movement.enabled = true;

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
