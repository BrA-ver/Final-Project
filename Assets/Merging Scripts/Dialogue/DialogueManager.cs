using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    InputHandler input;

    Dialogue dialogue;

    bool dialogueStarted;
    bool makingChoice;
    int index = 0;

    public event Action onDialogeStarted;
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

    public void EnterDialogue(Dialogue dialogue)
    {
        if (dialogueStarted) return;

        onDialogeStarted?.Invoke();
        dialogueStarted = true;
        this.dialogue = dialogue;

        ContinueOrExitDialogue();
        
    }

    private void ContinueOrExitDialogue()
    {
        if (index < dialogue.lines.Length)
        {
            string dialogueLine = dialogue.lines[index];
            Debug.Log(dialogueLine);
            onDisplayDialogue?.Invoke(dialogueLine);

            if (dialogue.evidence != null)
            {
                EvidenceManager.Instance.AddEvidence(dialogue.evidence);
            }

            if (index == dialogue.lines.Length - 1 && dialogue.choices.Length > 0)
            {
                Debug.Log("Choosing");
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
        Debug.Log("Exiting Dialogue");
        dialogueStarted = false;
        makingChoice = false;
        onDialogueFinished?.Invoke();
        onHodeChoices?.Invoke();
        index = 0;
        dialogue = null;

        // When the button is clicked, set the selected button to null
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void SelectChoice(DialogueChoice choice)
    {
        this.dialogue = choice.targetDialogue;
        makingChoice = false;
        index = 0;
        onHodeChoices?.Invoke();
        ContinueOrExitDialogue();

        // When the button is clicked, set the selected button to null
        EventSystem.current.SetSelectedGameObject(null);
    }

    void OnSubmit()
    {
        //Debug.Log("submit recieved");
        if (!dialogueStarted) return;

        ContinueOrExitDialogue();
    }
}
