using UnityEngine;
using TMPro;

public class DialogeDisplay : MonoBehaviour
{
    [SerializeField] GameObject displayHolder;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] DialogueChoiceButton[] choiceButtons;

    private void Awake()
    {
        displayHolder.gameObject.SetActive(false);
        dialogueText.text = string.Empty;
    }

    private void Start()
    {
        DialogueManager.Instance.onDialogeStarted += StartDialogue;
        DialogueManager.Instance.onDisplayDialogue += DisplayDialogue;
        DialogueManager.Instance.onDialogueFinished += StopDialogue;
        DialogueManager.Instance.onDisplayChoices += OnShowChoices;
        DialogueManager.Instance.onHodeChoices += OnHideChoices;

        foreach (DialogueChoiceButton button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        DialogueManager.Instance.onDialogeStarted -= StartDialogue;
        DialogueManager.Instance.onDisplayDialogue -= DisplayDialogue;
        DialogueManager.Instance.onDialogueFinished -= StopDialogue;
        DialogueManager.Instance.onDisplayChoices -= OnShowChoices;
        DialogueManager.Instance.onHodeChoices -= OnHideChoices;
    }

    void StartDialogue()
    {
        displayHolder.gameObject.SetActive(true);
    }

    void StopDialogue()
    {
        displayHolder.gameObject.SetActive(false);
        dialogueText.text = string.Empty;
    }

    public void DisplayDialogue(string line)
    {
        dialogueText.text = line;

        
    }

    public void OnShowChoices(DialogueChoice[] choices)
    {
        Debug.Log("Showing Choices");
        if (choices.Length > 0)
        {
            for (int i = 0; i < choices.Length; i++)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].SetChoice(choices[i]);
            }
            choiceButtons[0].Select();
        }
    }

    public void OnHideChoices()
    {
        foreach (DialogueChoiceButton button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }
}
 