using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using System;

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
    [field: SerializeField] public bool IsInterogating { get; set; }

    public event Action<Evidence> onEvidenceClick;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        HideDescription();
    }

    public void ToggleEvidence()
    {
        //if (DialogueManager.Instance.dialogueStarted) return;
        //if (ActionScreen.instance.performingAction && !IsInterogating) return;

        InteractionState currentState = GameManager.instance.CurrentState;
        InteractionState previousState = GameManager.instance.PreviousState;

        // OPEN THE EVIDENCE BOARD
        // - if we are in the free sate
        if (currentState == InteractionState.None || previousState == InteractionState.None)
        {
            // If the board is closed - Open the evidence board and free the mouse
            if (!IsOpen)
            {
                OpenEvidenceBoard();
                GameManager.instance.ShowMouse();
                GameManager.instance.SwitchState(InteractionState.EvidenceBoard);
            }
            // If it is open - Close the board and lock mouse
            else
            {
                CloseEvidenceBoard();
                GameManager.instance.HideMouse();
                GameManager.instance.SwitchState(InteractionState.None);
            }
        }
        else
        {
            // CLOSE THE BOARD
            CloseEvidenceBoard();

            // DO SOMETHNG DEPENDING ON THE PREVIOUS STATE
            // If the previous state is action screen - Open the action screen
            if (previousState == InteractionState.ActionScreen)
            {
                ActionScreen.instance.ShowActions();
            }
            // If the previos state is case file - Open the case file
            if (previousState == InteractionState.CaseFile)
            {
                CaseFile.instance.OpenCaseFile();
            }
        }


        //if (currentState != InteractionState.None && currentState != InteractionState.EvidenceBoard)
        //{
        //    return;
        //}

        //if (IsOpen)
        //{
        //    CloseEvidenceBoard();
        //    if (IsInterogating)
        //    {
        //        ActionScreen.instance.showActions = true;
        //        ActionScreen.instance.ShowActions();
        //        //Debug.Log("Actions Shown");
        //        IsInterogating = false;

        //        GameManager.instance.SwitchState(InteractionState.ActionScreen);
        //        return;
        //    }
        //    GameManager.instance.SwitchState(InteractionState.None);
        //}
        //else
        //{
        //    OpenEvidenceBoard();
        //}

        
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
        //StartCoroutine(SelectFirstSlot());
        GameManager.instance.ShowMouse();
    }

    public void CloseEvidenceBoard()
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
    }

    void ShowEvidence()
    {
        // Spawn evidence slot for each evidence
        foreach(Evidence evidence in evidences)
        {
            EvidenceSlot slot = Instantiate(slotPrefab, evidenceHolder);

            // Assign evidence to the slot
            slot.Initialize(evidence, this);

            // Add the slot to the active slots list
            activeSlots.Add(slot);

            slot.onSlotClick += OnSlotClick;
        }
    }

    private void OnSlotClick(Evidence evidence)
    {
        ShowDescription(evidence);
    }

    public void ShowDescription(Evidence evidence)
    {
        descrition.text = evidence.Description;
    }

    void HideDescription()
    {
        descrition.text = string.Empty;
    }

    public void Interogate()
    {
        IsInterogating = true;
        OpenEvidenceBoard();
    }

    public void StopInterogating()
    {
        IsInterogating = false;
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
        popupText.text = $"{clueName} collected";
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

    public void ClickEvidence(Evidence evidence)
    {
        onEvidenceClick?.Invoke(evidence);
    }
}
