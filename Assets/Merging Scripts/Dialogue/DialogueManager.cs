using System;
using UnityEngine;
//using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    InputHandler input;

    Dialogue dialogue;

    public bool dialogueStarted;
    bool makingChoice;
    int index = 0;

    bool isNpc;
    public NPC npc;

    bool openActionsAfterDialogue;

    public event Action onDialogeStarted; // Activates the dialoge box whenever dialogue is started
    public event Action onDialogueFinished; 
    public event Action<string> onDisplayDialogue;
    public event Action<DialogueChoice[]> onDisplayChoices;
    public event Action onHodeChoices;

    bool showedButtons;
    //----- WERNER ADDED -----
    public static event Action<Dialogue> OnDialogueStarted;
    //----- WERNER ADDED -----

    Dialogue lastChoice;

    private void Awake()
    {
        Instance = this;
        input = FindObjectOfType<InputHandler>();
    }

    private void OnEnable()
    {
        input.onSumbit += OnSubmit;
    }

    private void OnDisable()
    {
        input.onSumbit -= OnSubmit;
    }

    public void EnterDialogue(Dialogue dialogue, NPC npc = null)
    {
        if (dialogueStarted) return;

        if (npc != null)
        {
            isNpc = true;
            this.npc = npc;
        }
        //Debug.Log("Entering Dialogue");

        onDialogeStarted?.Invoke();
        dialogueStarted = true;
        this.dialogue = dialogue;

        //----- WERNER ADDED -----
        OnDialogueStarted?.Invoke(dialogue);
        //----- WERNER ADDED -----

        ContinueOrExitDialogue();
        GameManager.instance.SwitchState(InteractionState.Dialogue);
        GameManager.instance.ShowMouse();
    }

    private void ContinueOrExitDialogue()
    {
        if (!makingChoice)
        {
            //Debug.Log($"Display dailogue: {index < dialogue.lines.Length}");
            if (index < dialogue.lines.Length)
            {
                string dialogueLine = dialogue.lines[index];
                //Debug.Log(dialogueLine);
                onDisplayDialogue?.Invoke(dialogueLine);

                if (dialogue.evidence != null)
                {
                    EvidenceManager.Instance.AddEvidence(dialogue.evidence);
                }

                if (index == dialogue.lines.Length - 1 && dialogue.choices.Length > 0)
                {
                    //Debug.Log("Choosing");

                    makingChoice = true;
                    lastChoice = dialogue;
                }
                index++;
            }
            else
            {
                if (dialogue.isExit)
                {
                    ExitDialogue();
                }
                else
                {
                    ReturnToMainQuestions();
                }
            }
        }
        else
        {
            if (showedButtons) return;
            showedButtons = true;
            onDisplayDialogue?.Invoke(string.Empty);
            onDisplayChoices?.Invoke(dialogue.choices);
        }
    }

    public void ReturnToMainQuestions()
    {
        GameManager.instance.SwitchState(InteractionState.Dialogue);

        //Debug.Log("Exiting Dialogue");
        //dialogueStarted = false;
        makingChoice = false;
        //onDialogueFinished?.Invoke();
        onHodeChoices?.Invoke();
        index = 0;
        dialogue = null;

        if (!isNpc)
        {
            GameEvents.OnInteractStop();
        }

        if (isNpc)
        {
            dialogue = npc.Profile.MainDialogue;
            onDialogeStarted?.Invoke();
            onDisplayDialogue?.Invoke(string.Empty);
            onDisplayChoices?.Invoke(dialogue.choices);
            
            makingChoice = true;
            showedButtons = true;

            if (EvidenceDisplay.instance.isIntergating)
            {
                EvidenceDisplay.instance.OpenEvidenceBoard();
                GameManager.instance.SwitchState(InteractionState.EvidenceBoard);
                DialogeDisplay.instance.StopDialogue();
            }
        }

        //GameManager.instance.HideMouse();
        // When the button is clicked, set the selected button to null
        //DeselectButton();
    }

    void ExitDialogue()
    {
        dialogueStarted = false;
        onDialogueFinished?.Invoke();
        makingChoice = false;

        onHodeChoices?.Invoke();
        index = 0;
        dialogue = null;

        GameManager.instance.SwitchState(InteractionState.None);
        GameManager.instance.HideMouse();
    }

    public void SelectChoice(DialogueChoice choice)
    {
        if (choice.targetDialogue != null)
        {
            this.dialogue = choice.targetDialogue;
            makingChoice = false;
            showedButtons = false;
            index = 0;
            onHodeChoices?.Invoke();
            //DeselectButton();

            //----- WERNER ADDED -----
            OnDialogueStarted?.Invoke(choice.targetDialogue);
            //----- WERNER ADDED -----

            ContinueOrExitDialogue();

            // When the button is clicked, set the selected button to null
        }
        else
        {
            Debug.LogWarning("WARNING: There is no dialogue following this choice");
        }
        
    }

    public void ResponceDialogue(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        makingChoice = false;
        showedButtons = false;
        index = 0;
        onHodeChoices?.Invoke();
        //DeselectButton();

        //----- WERNER ADDED -----
        OnDialogueStarted?.Invoke(dialogue);
        //----- WERNER ADDED -----

        ContinueOrExitDialogue();
    }

    void OnSubmit()
    {
        //Debug.Log("submit recieved");
        if (!dialogueStarted || GameManager.instance.IgnoreMouseInput) return;

        if (GameManager.instance.CurrentState != InteractionState.Dialogue) return;

        ContinueOrExitDialogue();
    }

    public void ShowActionsAfterDialogue()
    {
        openActionsAfterDialogue = true;
    }

    //public void DeselectButton()
    //{
    //    // Get the currently selected button
    //    var selected = EventSystem.current.currentSelectedGameObject;

    //    // Trigger its OnDeselect (fires EventTrigger or IDeslectHandler)
    //    UIHelper.TriggerOnDeselect(selected);

    //    // Then clear selection
    //    EventSystem.current.SetSelectedGameObject(null);
    //}
}
