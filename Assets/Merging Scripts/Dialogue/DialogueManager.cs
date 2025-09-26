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

    bool openActionsAfterDialogue;

    public event Action onDialogeStarted; // Activates the dialoge box whenever dialogue is started
    public event Action onDialogueFinished; 
    public event Action<string> onDisplayDialogue;
    public event Action<DialogueChoice[]> onDisplayChoices;
    public event Action onHodeChoices;

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

    public void SwitchDialogue(Dialogue dialogue)
    {
        ExitDialogue();
        EnterDialogue(dialogue);
    }

    public void EnterDialogue(Dialogue dialogue, bool isNpc = false)
    {
        if (dialogueStarted) return;

         this.isNpc = isNpc;
        //Debug.Log("Entering Dialogue");

        onDialogeStarted?.Invoke();
        dialogueStarted = true;
        this.dialogue = dialogue;

        ContinueOrExitDialogue();
        
    }

    private void ContinueOrExitDialogue()
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
                onDisplayChoices?.Invoke(dialogue.choices);
                makingChoice = true;
            }
            index++;
        }
        else if (!makingChoice)
            ExitDialogue();
    }

    private void ExitDialogue()
    {
        //Debug.Log("Exiting Dialogue");
        dialogueStarted = false;
        makingChoice = false;
        onDialogueFinished?.Invoke();
        onHodeChoices?.Invoke();
        index = 0;
        dialogue = null;

        if (!isNpc)
        {
            GameEvents.OnInteractStop();
        }

        // When the button is clicked, set the selected button to null
        //DeselectButton();
    }

    public void SelectChoice(DialogueChoice choice)
    {
        this.dialogue = choice.targetDialogue;
        makingChoice = false;
        index = 0;
        onHodeChoices?.Invoke();
        //DeselectButton();
        ContinueOrExitDialogue();

        // When the button is clicked, set the selected button to null
        
    }

    void OnSubmit()
    {
        //Debug.Log("submit recieved");
        if (!dialogueStarted) return;

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
