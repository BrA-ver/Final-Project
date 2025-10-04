using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ActionScreen : MonoBehaviour
{
    public static ActionScreen instance;

    [SerializeField] GameObject holder;
    [SerializeField] Button firstButton;

    NPC interactedNPC;

    public bool showActions;

    public bool performingAction = false;

    public event Action onExitActions;

    private void Awake()
    {
        instance = this;
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
        EvidenceDisplay.instance.IsInterogating = false;

        showActions = true;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Question()
    {
        //DeselectButton();
        HideActions();
        EvidenceDisplay.instance.Interogate();

        showActions = true;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Exit()
    {
        //DeselectButton();
        GameEvents.OnInteractStop();
        HideActions();
        EventSystem.current.SetSelectedGameObject(null);

        performingAction = false;
        onExitActions?.Invoke();

        GameManager.instance.SwitchState(InteractionState.None);
    }

    public void ShowActions()
    {
        if (!showActions) return;
        holder.SetActive(true);
        Invoke(nameof(SelectFirst), .5f);

        performingAction = true;

        GameManager.instance.SwitchState(InteractionState.ActionScreen);
    }

    public void ShowActions(NPC npc)
    {
        holder.SetActive(true);
        Invoke(nameof(SelectFirst), .5f);
        interactedNPC = npc;

        performingAction = true;

        GameManager.instance.SwitchState(InteractionState.ActionScreen);
    }

    void SelectFirst()
    {
        //Debug.Log("Selected Button");
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
