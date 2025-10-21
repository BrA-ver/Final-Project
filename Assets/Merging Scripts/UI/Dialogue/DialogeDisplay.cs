using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogeDisplay : MonoBehaviour
{
    public static DialogeDisplay instance;

    [SerializeField] GameObject displayHolder;
    [SerializeField] TextMeshProUGUI dialogueText;
    List<DialogueChoiceButton> choiceButtons =  new List<DialogueChoiceButton>();

    [SerializeField] Transform buttonHolder;

    [Header("NPC Dialogue")]
    [SerializeField] GameObject profileHolder;
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI nameText;

    private void Awake()
    {
        instance = this;

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

        foreach (Transform button in buttonHolder)
        {
            if (button.TryGetComponent<DialogueChoiceButton>(out DialogueChoiceButton choiceButton))
            {
                choiceButtons.Add(choiceButton);
            }
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
            //DialogueManager.Instance.DeselectButton();
        }
    }

    public void OnHideChoices()
    {
        foreach (DialogueChoiceButton button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void ShowCharacterProfile(ProfileSO profile)
    {
        profileHolder.SetActive(true);
        characterImage.sprite = profile.picture;
        nameText.text = profile._name;
    }
}
 