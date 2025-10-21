using UnityEngine;
using System.Collections;

public class DialogueTriggerAction : MonoBehaviour
{
    [Header("Target Dialogue Name")]
    [SerializeField] private string triggerDialogueName = "Escort to Body";

    [Header("Optional: Reference to AI Script")]
    [SerializeField] private PoliceNavMesh policeNavMesh;

    private void Awake()
    {
        if (policeNavMesh == null)
            policeNavMesh = GetComponent<PoliceNavMesh>();
    }

    private void OnEnable()
    {
        DialogueManager.OnDialogueStarted += HandleDialogueStart;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueStarted -= HandleDialogueStart;
    }

    private void HandleDialogueStart(Dialogue dialogue)
    {
        if (dialogue == null || policeNavMesh == null) return;

        if (dialogue.name.Equals(triggerDialogueName, System.StringComparison.OrdinalIgnoreCase))
        {
            policeNavMesh.StartPatrol();

            // Close dialogue & re-enable player
            StartCoroutine(CloseDialogueAndActions());
        }
    }

    private IEnumerator CloseDialogueAndActions()
    {
        // Wait one frame to avoid timing issues
        yield return null;

        // Close the Dialogue (same as pressing Exit)
        var exitMethod = typeof(DialogueManager).GetMethod(
            "ExitDialogue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );

        if (exitMethod != null && DialogueManager.Instance != null)
        {
            exitMethod.Invoke(DialogueManager.Instance, null);
        }

        // Close the ActionScreen (Talk / Interrogate menu)
        if (ActionScreen.instance != null)
        {
            ActionScreen.instance.HideActions();
        }

        // Re-enable player movement
        if (GameManager.instance != null)
        {
            GameManager.instance.HideMouse();
            GameManager.instance.SwitchState(InteractionState.None);
        }
    }
}
