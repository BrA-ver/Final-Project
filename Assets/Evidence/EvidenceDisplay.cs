using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class EvidenceDisplay : MonoBehaviour
{
    public static EvidenceDisplay instance;

    [Header("Display")]
    [SerializeField] GameObject Display;
    [SerializeField] TextMeshProUGUI descrition;

    [Header("Evidence Slots")]
    [SerializeField] EvidenceSlot slotPrefab;
    [SerializeField] RectTransform evidenceHolder;
    [SerializeField] int maxSlots = 12;

    [Header("Pop Up")]
    [SerializeField] GameObject popupObj;
    [SerializeField] TextMeshProUGUI popupText;
    [SerializeField] float popUpTime = 1f;

    List<EvidenceSlot> activeSlots = new List<EvidenceSlot>();

    List<Evidence> evidences;

    public bool IsOpen { get; private set; }
    public bool IsInterogating { get; set; }

    private void Awake()
    {
        instance = this;
    }

    public void ToggleEvidence()
    {
        if (DialogueManager.Instance.dialogueStarted) return;
        if (ActionScreen.instance.performingAction && !IsInterogating) return;

        if (IsOpen)
        {
            CloseEvidenceBoard(false);
            if (IsInterogating)
            {
                ActionScreen.instance.showActions = true;
                ActionScreen.instance.ShowActions();
                //Debug.Log("Actions Shown");
            }
        }
        else
            OpenEvidenceBoard();
    }

    public void OpenEvidenceBoard()
    {
        //Debug.Log("Opening Evidence Board");
        IsOpen = true;
        Display.SetActive(true);

        // Lists are not duplicatable with simple assignment,
        List<Evidence> allEvidences = EvidenceManager.Instance.AllEvidence;
        evidences = new List<Evidence>(allEvidences);
        ShowEvidence();

        //SelectSlot();
        GameManager.instance.StartInteracting();
        StartCoroutine(SelectFirstSlot());
    }

    public void CloseEvidenceBoard(bool interogating)
    {
        //Debug.Log("Closing Evidence Board");
        IsOpen = false;
        Display.SetActive(false);
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
        
    }

    void ShowEvidence()
    {
        // Spawn evidence slot for each evidence
        foreach(Evidence evidence in evidences)
        {
            EvidenceSlot slot = Instantiate(slotPrefab, evidenceHolder);

            // Assign evidence to the slot
            slot.Initialize(evidence);

            // Add the slot to the active slots list
            activeSlots.Add(slot);
        }
    }

    public void ShowDescription(EvidenceSlot slot)
    {
        descrition.text = slot.Evidence.Description;
    }

    public void Interogate()
    {
        IsInterogating = true;
        OpenEvidenceBoard();
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

    IEnumerator SelectFirstSlot()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        
        if (activeSlots.Count > 0)
            EventSystem.current.SetSelectedGameObject(activeSlots[0].gameObject);
    }
}
