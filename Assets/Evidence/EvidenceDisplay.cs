using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class EvidenceDisplay : MonoBehaviour
{
    public static EvidenceDisplay instance;

    [SerializeField] EvidenceSlot slotPrefab;
    [SerializeField] RectTransform evidenceHolder;
    [SerializeField] GameObject parent;

    [SerializeField] int maxSlots = 12;
    List<EvidenceSlot> activeSlots = new List<EvidenceSlot>();

    List<Evidence> evidences;

    // Selecting Slots
    [SerializeField] int index = 0;
    EvidenceSlot selectedSlot;

    public bool IsOpen;
    public bool IsInterogating { get; set; }

    [Header("Description")]
    [SerializeField] TextMeshProUGUI descrition;

    [Header("Pop Up")]
    [SerializeField] GameObject popupObj;
    [SerializeField] TextMeshProUGUI popupText;
    [SerializeField] float popUpTime = 1f;

    private void Awake()
    {
        instance = this;
    }

    public void ToggleEvidence()
    {
        if (DialogueManager.Instance.dialogueStarted) return;
        if (ActionScreen.instance.performingAction && !IsInterogating) return;

        if (IsOpen)
            CloseEvidenceBoard();
        else
            OpenEvidenceBoard();
    }

    public void OpenEvidenceBoard()
    {
        Debug.Log("Opening Evidence Board");
        IsOpen = true;
        parent.SetActive(true);

        // Lists are not duplicatable with simple assignment,
        List<Evidence> allEvidences = EvidenceManager.Instance.AllEvidence;
        evidences = new List<Evidence>(allEvidences);
        ShowEvidence();
        index = 0;

        SelectSlot();
        GameManager.instance.StartInteracting();
    }

    public void CloseEvidenceBoard()
    {
        Debug.Log("Closing Evidence Board");
        IsOpen = false;
        parent.SetActive(false);
        evidences.Clear();
        
        foreach (EvidenceSlot slot in activeSlots)
        {
            Destroy(slot.gameObject);
        }
        activeSlots.Clear();

        if (!IsInterogating)
        {
            GameManager.instance.StopInteracting();
        }
        else
        {
            ActionScreen.instance.ShowActions();
        }
    }

    void ShowEvidence()
    {
        // Spawn evidence slot for each evidence
        foreach(Evidence evidence in evidences)
        {
            EvidenceSlot slot = Instantiate(slotPrefab, evidenceHolder);

            // Assign evidence to the slot
            slot.SetEvidence(evidence);

            // Add the slot to the active slots list
            activeSlots.Add(slot);
        }
    }

    void SelectSlot()
    {
        if (activeSlots.Count <= 0) return;
        if (selectedSlot)
            selectedSlot.Deselect();

        selectedSlot = activeSlots[index];
        selectedSlot.Select();

        ShowDescription();
    }

    void ShowDescription()
    {
        descrition.text = selectedSlot.Evidence.Description;
    }

    public void ToggleSlot(float slot)
    {
        if (slot > 0.1)
        {
            index++;
            if (index >= activeSlots.Count)
                index = 0;
        }
        else if (slot < -0.1)
        {
            index--;
            if (index < 0)
                index = activeSlots.Count - 1;
        }

        SelectSlot();
    }

    #region Pop Up
    public void ShowPopUp(string clueName)
    {
        StopCoroutine(PopUpRoutine(clueName));
        StartCoroutine(PopUpRoutine(clueName));
    }

    IEnumerator PopUpRoutine(string clueName)
    {
        popupObj.SetActive(true);
        popupText.text = $"{clueName} added to inventory";
        yield return new WaitForSeconds(popUpTime);

        popupText.text = string.Empty;
        popupObj.SetActive(false);
    }
    #endregion
}
