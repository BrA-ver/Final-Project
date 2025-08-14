using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    bool dialogueStarted;

    private void Awake()
    {
        Instance = this;
    }

    public void EnterDialogue()
    {
        if (dialogueStarted) return;

        dialogueStarted = true;
        Debug.Log("Entered Dialogue");
    }
}
