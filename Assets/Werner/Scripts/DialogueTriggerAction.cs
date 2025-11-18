using UnityEngine;

public class DialogueTriggerAction : MonoBehaviour
{
    [Header("Trigger Dialogue Name")]
    [SerializeField] private string triggerDialogueName = "Escort to Body";

    [Header("AI Reference")]
    [SerializeField] private PoliceNavMesh policeNavMesh;

    private bool waitingForFinalLine = false;

    private void Awake()
    {
        if (policeNavMesh == null)
            policeNavMesh = GetComponent<PoliceNavMesh>();
    }

    private void OnEnable()
    {
        DialogueManager.OnDialogueStarted += OnDialogueStarted;
        DialogueManager.OnDialogueLineDisplayed += OnLineDisplayed;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueStarted -= OnDialogueStarted;
        DialogueManager.OnDialogueLineDisplayed -= OnLineDisplayed;
    }

    private void OnDialogueStarted(Dialogue dialogue)
    {
        if (dialogue == null) return;

        if (dialogue.name.Equals(triggerDialogueName, System.StringComparison.OrdinalIgnoreCase))
        {
            waitingForFinalLine = true;
        }
    }

    private void OnLineDisplayed(Dialogue dialogue, int lineIndex)
    {
        if (!waitingForFinalLine) return;
        if (dialogue == null) return;

        if (!dialogue.name.Equals(triggerDialogueName, System.StringComparison.OrdinalIgnoreCase))
            return;

        if (lineIndex == dialogue.lines.Length - 1)
        {
            waitingForFinalLine = false;

            StartEscortSequence();
        }
    }

    private void StartEscortSequence()
    {
        policeNavMesh.StartPatrol();

        DialogueManager.Instance.SendMessage("ExitDialogue", SendMessageOptions.DontRequireReceiver);

        if (ActionScreen.instance != null)
            ActionScreen.instance.HideActions();

        GameManager.instance.HideMouse();
        GameManager.instance.SwitchState(InteractionState.None);
    }
}
