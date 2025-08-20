using UnityEngine;

public class DialogueHolder : Interactable
{
    [SerializeField] protected Dialogue[] dialogues;

    public override void Interact()
    {
        base.Interact();
        DialogueManager.Instance.EnterDialogue(dialogues[0]);

    }
}
