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

    private CharacterController controller;
    private PlayerMovement movement;
    private bool isReturning = false;
    private bool playerInRift = false;

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
    }

    void Update()
    {
        if (player == null || isReturning) return;

        // Only check if player is actually in Rift (Map 2)
        if (!playerInRift && player.position.z > 100f) // threshold ~ halfway to Map2
        {
            playerInRift = true;
        }

        if (playerInRift)
        {
            float dist = Vector3.Distance(player.position, transform.position);

            if (dist > maxDistance)
            {
                Debug.Log($"Player left rift bubble! Distance = {dist}");
                StartCoroutine(ReturnToMap1());
            }
        }
    }

    IEnumerator ReturnToMap1()
    {
        isReturning = true;
        if (movement != null) movement.enabled = false;

        PlayEffects();

        yield return new WaitForSeconds(delayBeforeReturn);

        Vector3 pos = player.position;
        pos = new Vector3(pos.x, pos.y, pos.z - mapOffsetZ); // one jump back to Map 1

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

        Debug.Log("Player teleported back to Map 1!");

        // Reset state so it won't trigger again until player re-enters Map 2
        playerInRift = false;
        isReturning = false;
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
