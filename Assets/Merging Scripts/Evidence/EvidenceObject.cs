using UnityEngine;

public class EvidenceObject : Interactable
{
    [SerializeField] Evidence evidence;
    [SerializeField] bool disappearAfterCollection;

    public override void Interact()
    {
        base.Interact();
        EvidenceManager.Instance.AddEvidence(evidence);
        if (disappearAfterCollection)
            gameObject.SetActive(false);
    }
}
