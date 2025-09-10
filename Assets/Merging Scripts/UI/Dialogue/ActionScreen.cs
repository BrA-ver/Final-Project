using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionScreen : MonoBehaviour
{
    public static ActionScreen instance;

    [SerializeField] GameObject holder;
    [SerializeField] Button firstButton;

    NPC interactedNPC;

    bool showActions;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DialogueManager.Instance.onDialogueFinished += ShowActions;
    }

    private void OnDisable()
    {
        DialogueManager.Instance.onDialogueFinished -= ShowActions;
    }

    public void ShowActions(NPC npc)
    {
        interactedNPC = npc;

        holder.SetActive(true);
        firstButton.Select();
    }

    public void HideActions()
    {
        holder.SetActive(false);
        showActions = false;
        //firstButton.Select();
    }

    public void Talk()
    {
        //DeselectButton();
        HideActions();
        interactedNPC.Talk();

        showActions = true;
    }

    public void Question()
    {
        //DeselectButton();
        HideActions();
        EvidenceDisplay.instance.OpenEvidenceBoard();

        showActions = true;
    }

    public void Exit()
    {
        //DeselectButton();
        GameEvents.OnInteractStop();
        HideActions();
    }

    void ShowActions()
    {
        if (!showActions) return;
        holder.SetActive(true);
        Invoke(nameof(SelectFirst), .5f);
    }

    void SelectFirst()
    {
        firstButton.Select();
    }

    //void DeselectButton()
    //{
    //    // Get the currently selected button
    //    var selected = EventSystem.current.currentSelectedGameObject;

    //    // Trigger its OnDeselect (fires EventTrigger or IDeslectHandler)
    //    UIHelper.TriggerOnDeselect(selected);

    //    // Then clear selection
    //    EventSystem.current.SetSelectedGameObject(null);
    //}
}
