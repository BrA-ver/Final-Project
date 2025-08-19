using System;
using UnityEngine;

public class NPC : DialogueHolder
{
    [Header("Evidence")]
    [SerializeField] Evidence evidence;
    [SerializeField] Dialogue responseDialoge;
    [SerializeField] Dialogue unresponseDialoge;

    private void OnDisable()
    {
        EvidenceManager.Instance.onPresentEvidence -= OnPresentEvidence;
    }

    public override void Interact()
    {
        base.Interact();
        EvidenceManager.Instance.onPresentEvidence += OnPresentEvidence;
    }

    private void OnPresentEvidence(Evidence evidence)
    {
        if (evidence == this.evidence)
        {
            Debug.Log("How did you get that?");
            DialogueManager.Instance.SwitchDialogue(responseDialoge);
        }
        else
        {
            Debug.Log("Is that supposed to mean something?");
        }
    }
}
