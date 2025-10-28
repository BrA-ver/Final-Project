using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueChoiceButton : MonoBehaviour, IScrollHandler
{
    [SerializeField] Button button;
    [SerializeField] private TextMeshProUGUI choiceText;
    [SerializeField] DialogueChoice choice;
    bool selected;

    [Header("Scrolling")]
    [SerializeField] ScrollRect scrollRect;

    public void SetChoice(DialogueChoice choice, ScrollRect rect)
    {
        this.choice = choice;
        choiceText.text = choice.text;
        scrollRect = rect;
    }

    public void Select()
    {
        StartCoroutine(SelectRoutine());
    }

    public void Deselect()
    {
        selected = false;
    }

    IEnumerator SelectRoutine()
    {
        yield return null;
        button.Select();
        selected = true;
    }


    public void SelectChoice()
    {
        
        if (choice == null)
        {
            Debug.Log("No choice to choose");
            return;
        }
        DialogueManager.Instance.SelectChoice(choice);
    }

    public void OnScroll(PointerEventData eventData)
    {
        ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.scrollHandler);
    }
}
