using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueChoiceButton : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] private TextMeshProUGUI choiceText;
    [SerializeField] DialogueChoice choice;
    bool selected;

    private void Start()
    {
        
    }

    public void SetChoice(DialogueChoice choice)
    {
        this.choice = choice;
        choiceText.text = choice.text;
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
}
