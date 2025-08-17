using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimeTravel : MonoBehaviour
{
    public static TimeTravel Instance;

    [Header("Scene Settings")]
    public string targetSceneName = "NewScene";
    public float delayBeforeTeleport = 2f;

    [Header("Assign particle system instances")]
    public ParticleSystem effect1;
    public ParticleSystem effect2;
    public ParticleSystem effect3;

    private bool isTeleporting = false;

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

        PlayEffects();

        yield return new WaitForSeconds(delayBeforeTeleport);

        StopEffects();
        DeactivateEffects();

        if (effect1 != null) Destroy(effect1.gameObject);
        if (effect2 != null) Destroy(effect2.gameObject);
        if (effect3 != null) Destroy(effect3.gameObject);

        SceneManager.LoadScene(targetSceneName);

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

