using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogeDisplay : MonoBehaviour
{
    public static DialogeDisplay instance;

    [SerializeField] GameObject displayHolder;
    [SerializeField] TextMeshProUGUI dialogueText;

    [SerializeField] DialogueChoiceButton choiceButton;
    List<DialogueChoiceButton> choiceButtons =  new List<DialogueChoiceButton>();

    [SerializeField] Transform buttonHolder;

    [Header("NPC Dialogue")]
    [SerializeField] GameObject profileHolder;
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI nameText;

    [SerializeField] ScrollRect rect;

    [SerializeField] Image mouseIcon;

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
    }

    private void OnDisable()
    {
        DialogueManager.Instance.onDialogeStarted -= StartDialogue;
        DialogueManager.Instance.onDisplayDialogue -= DisplayDialogue;
        DialogueManager.Instance.onDialogueFinished -= StopDialogue;
        DialogueManager.Instance.onDisplayChoices -= OnShowChoices;
        DialogueManager.Instance.onHodeChoices -= OnHideChoices;
    }

    public void StartDialogue()
    {
        displayHolder.gameObject.SetActive(true);
    }

    public void StopDialogue()
    {
        displayHolder.gameObject.SetActive(false);
        dialogueText.text = string.Empty;
    }

    public void DisplayDialogue(string line)
    {
        dialogueText.text = line;

        if (line == string.Empty)
        {
            mouseIcon.gameObject.SetActive(false);
        }
        else
        {
            mouseIcon.gameObject.SetActive(true);
        }
    }

    public void OnShowChoices(DialogueChoice[] choices)
    {
       
        //Debug.Log("Showing Choices");
        //if (choices.Length > 0)
        //{
        //    for (int i = 0; i < choices.Length; i++)
        //    {
        //        choiceButtons[i].gameObject.SetActive(true);
        //        choiceButtons[i].SetChoice(choices[i]);
        //    }
        //    //DialogueManager.Instance.DeselectButton();
        //}

        if (choices.Length > 0)
        {
            foreach (DialogueChoice choice in choices)
            {
                DialogueChoiceButton button = Instantiate(choiceButton, buttonHolder);
                button.SetChoice(choice, rect);
                choiceButtons.Add(button);
            }
        }
    }

    public void OnHideChoices()
    {
        foreach (DialogueChoiceButton button in choiceButtons)
        {
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();
    }

    public void ShowCharacterProfile(ProfileSO profile)
    {
        profileHolder.SetActive(true);
        characterImage.sprite = profile.picture;
        nameText.text = profile._name;
    }

    #region Interogation

    public void Interogate()
    {
        EvidenceDisplay.instance.OpenEvidenceBoard();
        StopDialogue();
        EvidenceDisplay.instance.isIntergating = true;
        GameManager.instance.SwitchState(InteractionState.EvidenceBoard);

    }

    public void IgnoreMouseInput(bool ignore) // Called by the interact button when the mouse enters it
    {
        GameManager.instance.IgnoreMouseInput = ignore;
    }

    #endregion
}
