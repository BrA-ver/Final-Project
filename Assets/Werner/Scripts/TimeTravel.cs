using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TimeTravel : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private float mapOffsetZ = 200f; 
    [SerializeField] private float delayBeforeTeleport = 2f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode teleportKey = KeyCode.Q;

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
    [SerializeField] private Text popupLabel; // ✅ NEW: reference to the popup text

    private PlayerMovement cachedMovement;
    private CharacterController controller;
    private CamerTarget cameraLook; // your camera look script

    private Dictionary<MeshRenderer, Material[]> originalMats = new Dictionary<MeshRenderer, Material[]>();
    private MeshRenderer activeCrystal = null;
    private bool isTeleporting = false;
    private bool popupActive = false;
    private bool pendingFromMap1;

    void Awake()
    {
        cachedMovement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        cameraLook = GetComponentInChildren<CamerTarget>();

        foreach (var c in map1Crystals)
            if (c != null) originalMats[c] = c.materials;
        foreach (var c in map2Crystals)
            if (c != null) originalMats[c] = c.materials;

        if (teleportPopup != null) teleportPopup.SetActive(false);
        if (yesButton != null) yesButton.onClick.AddListener(OnYesClicked);
        if (noButton != null) noButton.onClick.AddListener(OnNoClicked);
    }

    void Update()
    {
        if (popupActive) return;

        MeshRenderer nearest = GetNearestCrystal();
        bool inRange = nearest != null;

        if (inRange && activeCrystal != nearest)
        {
            ClearHighlight();
            HighlightCrystal(nearest);
        }
        else if (!inRange && activeCrystal != null)
        {
            ClearHighlight();
        }

        if (inRange && !isTeleporting && Input.GetKeyDown(teleportKey))
        {
            pendingFromMap1 = map1Crystals.Contains(nearest);
            ShowPopup();
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

    // --- UI logic ---
    void ShowPopup()
    {
        popupActive = true;
        if (teleportPopup != null) teleportPopup.SetActive(true);

        if (cachedMovement != null) cachedMovement.enabled = false;
        if (cameraLook != null) cameraLook.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // ✅ Change popup text depending on crystal
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
        if (cachedMovement != null) cachedMovement.enabled = false;

        PlayEffects();
        yield return new WaitForSeconds(delayBeforeTeleport);

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

        StopEffects();
        DeactivateEffects();

        if (cachedMovement != null) cachedMovement.enabled = true;
        if (cameraLook != null) cameraLook.enabled = true;

        // ✅ Tell RiftCrystalReturn to ignore auto-teleport briefly
        var returnScript = FindObjectOfType<RiftCrystalReturn>();
        if (returnScript != null)
        {
            returnScript.SuppressReturn(2f);

            // ✅ Extra: Reset Map 2 state when we teleport back to Map 1
            if (!fromMap1)
            {
                returnScript.ForceExitRift();
            }
        }

        isTeleporting = false;
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
