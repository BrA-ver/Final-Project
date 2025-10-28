using System;
using System.Collections.Generic;
using UnityEngine;

public class NPC : DialogueHolder
{
    [Header("Character Profile")]
    [SerializeField] ProfileSO profile;



    public override void Interact()
    {
        base.Interact();
        DialogueManager.Instance.EnterDialogue(dialogues[0]);
    }

    //[SerializeField] List<EvidenceResponce> evidenceResponses;
    //[SerializeField] Dialogue nullResponse;

    //bool responding;

    //private void OnDisable()
    //{
    //    EvidenceManager.Instance.onPresentEvidence -= OnPresentEvidence;
    //}

    //public override void Interact()
    //{
    //    //base.Interact();

    //    EvidenceManager.Instance.onPresentEvidence += OnPresentEvidence;
    //    ActionScreen.instance.ShowActions(this);

    //    ActionScreen.instance.onExitActions += OnExitActions;
    //}

    //private void OnExitActions()
    //{
    //    EvidenceManager.Instance.onPresentEvidence -= OnPresentEvidence;
    //    ActionScreen.instance.onExitActions -= OnExitActions;
    //}

    //public void Talk()
    //{
    //    //EvidenceManager.Instance.onPresentEvidence += OnPresentEvidence;
    //    DialogueManager.Instance.EnterDialogue(dialogues[0], true);
    //    DialogueManager.Instance.onDialogueFinished += OnDialogueFinished;

    //    DialogeDisplay.instance.ShowCharacterProfile(profile);
    //}

    //private void OnPresentEvidence(Evidence evidence)
    //{
    //    Debug.Log("Present Evidence");
    //    Dialogue responseDialogue = null;
    //    if (HasResponce(evidence, out responseDialogue))
    //    {
    //        RespondToEvidence(responseDialogue);
    //    }
    //    else
    //    {
    //        RespondToEvidence(nullResponse);
    //    }
    //}

    //void RespondToEvidence(Dialogue dialogue)
    //{
    //    responding = true;
    //    DialogueManager.Instance.EnterDialogue(dialogue, true);

    //    ActionScreen.instance.HideActions();
    //    DialogueManager.Instance.onDialogueFinished += OnDialogueFinished;
    //}

    //private void OnDialogueFinished()
    //{
    //    Debug.Log("Dialogue End");
    //    if (responding)
    //    {
    //        responding = false;
    //        EvidenceDisplay.instance.OpenEvidenceBoard();
    //    }
    //    else
    //    {
    //        ActionScreen.instance.ShowActions(this);
    //    }
    //    DialogueManager.Instance.onDialogueFinished -= OnDialogueFinished;
    //}

    //bool HasResponce(Evidence evidence, out Dialogue responseDialogue)
    //{
    //    bool hasResponse = false;
    //    responseDialogue = null;
    //    foreach (EvidenceResponce responce in evidenceResponses)
    //    {
    //        if (responce.evidence == evidence) 
    //        {
    //            responseDialogue = responce.response;
    //            hasResponse = true; 
    //        }
    //    }

    //    return hasResponse;
    //}
}

[System.Serializable]
public class EvidenceResponce
{
    public Evidence evidence;
    public Dialogue response;
}