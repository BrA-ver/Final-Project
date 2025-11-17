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

        // Mark that THIS dialogue's final line should trigger the escort
        if (dialogue.name.Equals(triggerDialogueName, System.StringComparison.OrdinalIgnoreCase))
        {
            waitingForFinalLine = true;
        }
    }

    private void OnLineDisplayed(Dialogue dialogue, int lineIndex)
    {
        if (!waitingForFinalLine) return;
        if (dialogue == null) return;

        // Ensure this is the correct dialogue
        if (!dialogue.name.Equals(triggerDialogueName, System.StringComparison.OrdinalIgnoreCase))
            return;

        // Check if this is the LAST line of this dialogue
        if (lineIndex == dialogue.lines.Length - 1)
        {
            waitingForFinalLine = false;

            // Execute the escort action
            StartEscortSequence();
        }
    }

    private void StartEscortSequence()
    {
        // Start escort movement
        policeNavMesh.StartPatrol();

        // Close Dialogue UI
        DialogueManager.Instance.SendMessage("ExitDialogue", SendMessageOptions.DontRequireReceiver);

        // Hide action buttons too
        if (ActionScreen.instance != null)
            ActionScreen.instance.HideActions();

        // Return player control
        GameManager.instance.HideMouse();
        GameManager.instance.SwitchState(InteractionState.None);
    }
}
