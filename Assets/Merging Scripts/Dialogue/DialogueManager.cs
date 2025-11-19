using System;
using UnityEngine;

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
    bool ignoreClick;

    public event Action onDialogeStarted;
    public event Action onDialogueFinished;
    public event Action<string> onDisplayDialogue;
    public event Action<DialogueChoice[]> onDisplayChoices;
    public event Action onHodeChoices;

    bool showedButtons;

    public static event Action<Dialogue> OnDialogueStarted;

    public static event Action<Dialogue, int> OnDialogueLineDisplayed;

    Dialogue lastChoice;

    [Header("Voice Acting")]
    public AudioSource voiceSource;
    public bool autoAdvanceWhenVoiceEnds = false;
    private bool waitingForVoiceToFinish = false;


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

    void Update()
    {
        if (autoAdvanceWhenVoiceEnds && waitingForVoiceToFinish)
        {
            if (!voiceSource.isPlaying)
            {
                waitingForVoiceToFinish = false;
                ContinueOrExitDialogue();
            }
        }
    }

    public void EnterDialogue(Dialogue dialogue, NPC npc = null)
    {
        if (dialogueStarted) return;

        if (npc != null)
        {
            isNpc = true;
            this.npc = npc;
        }

        onDialogeStarted?.Invoke();
        dialogueStarted = true;
        ignoreClick = true;
        this.dialogue = dialogue;

        OnDialogueStarted?.Invoke(dialogue);

        ContinueOrExitDialogue();
        GameManager.instance.SwitchState(InteractionState.Dialogue);
        GameManager.instance.ShowMouse();
    }

    private void ContinueOrExitDialogue()
    {
        if (!makingChoice)
        {
            if (index < dialogue.lines.Length)
            {
                string dialogueLine = dialogue.lines[index];
                Debug.Log(dialogueLine);

                onDisplayDialogue?.Invoke(dialogueLine);

                PlayVoiceLine(dialogue, index);

                OnDialogueLineDisplayed?.Invoke(dialogue, index);

                if (dialogue.evidence != null)
                {
                    EvidenceManager.Instance.AddEvidence(dialogue.evidence);
                }

                if (index == dialogue.lines.Length - 1 && dialogue.choices.Length > 0)
                {
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
                    EvidenceDisplay.instance.StopInteracting();
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

    private void PlayVoiceLine(Dialogue dialogue, int lineIndex)
    {
        if (voiceSource == null) return;
        if (dialogue.voiceLines == null) return;

        if (lineIndex < dialogue.voiceLines.Length &&
            dialogue.voiceLines[lineIndex] != null)
        {
            voiceSource.Stop();
            voiceSource.clip = dialogue.voiceLines[lineIndex];
            voiceSource.Play();

            if (autoAdvanceWhenVoiceEnds)
                waitingForVoiceToFinish = true;
        }
        else
        {
            waitingForVoiceToFinish = false;
        }
    }

    public void ReturnToMainQuestions()
    {
        GameManager.instance.SwitchState(InteractionState.Dialogue);

        makingChoice = false;
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

            OnDialogueStarted?.Invoke(choice.targetDialogue);

            ContinueOrExitDialogue();
        }
    }

    public void ResponceDialogue(Dialogue dialogue)
    {
        this.dialogue = dialogue;
        makingChoice = false;
        showedButtons = false;
        index = 0;
        onHodeChoices?.Invoke();

        OnDialogueStarted?.Invoke(dialogue);

        ContinueOrExitDialogue();
    }

    void OnSubmit()
    {
        if (ignoreClick)
        {
            ignoreClick = false;
            return;
        }

        if (!dialogueStarted || GameManager.instance.IgnoreMouseInput) return;
        if (GameManager.instance.CurrentState != InteractionState.Dialogue) return;

        if (voiceSource != null && voiceSource.isPlaying)
        {
            voiceSource.Stop();
        }

        waitingForVoiceToFinish = false;
        ContinueOrExitDialogue();
    }

    public void ShowActionsAfterDialogue()
    {
        openActionsAfterDialogue = true;
    }
}
