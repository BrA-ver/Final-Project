using UnityEngine;

public class EvidenceObject : Interactable
{
    [SerializeField] Evidence evidence;

    public override void Interact()
    {
        base.Interact();
        EvidenceManager.Instance.AddEvidence(evidence);
    }
}
