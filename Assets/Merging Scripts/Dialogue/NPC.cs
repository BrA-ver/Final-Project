using System.Collections.Generic;
using UnityEngine;

public class NPC : DialogueHolder
{
    [Header("Evidence")]
    [SerializeField] List<EvidenceResponce> evidenceResponses;
    [SerializeField] Dialogue nullResponse;

    private void OnDisable()
    {
        EvidenceManager.Instance.onPresentEvidence -= OnPresentEvidence;
    }

    public override void Interact()
    {
        //base.Interact();
        //EvidenceManager.Instance.onPresentEvidence += OnPresentEvidence;

        ActionScreen.instance.ShowActions();
    }

    private void OnPresentEvidence(Evidence evidence)
    {
        Dialogue responseDialogue = null;
        if (HasResponce(evidence, out responseDialogue))
        {
            DialogueManager.Instance.SwitchDialogue(responseDialogue);
        }
        else
        {
            DialogueManager.Instance.SwitchDialogue(nullResponse);
        }
    }

    bool HasResponce(Evidence evidence, out Dialogue responseDialogue)
    {
        bool hasResponse = false;
        responseDialogue = null;
        foreach (EvidenceResponce responce in evidenceResponses)
        {
            if (responce.evidence == evidence) 
            {
                responseDialogue = responce.response;
                hasResponse = true; 
            }
        }

        return hasResponse;
    }
}

[System.Serializable]
public class EvidenceResponce
{
    public Evidence evidence;
    public Dialogue response;
}