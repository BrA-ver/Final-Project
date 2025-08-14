using UnityEngine;

public class DialogueHolder : Interactable
{
    [SerializeField] Dialogue[] dialogues;

    public override void Interact()
    {
        base.Interact();
        DialogueManager.Instance.EnterDialogue();

    }
}
