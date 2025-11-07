using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class CaseFile : MonoBehaviour
{
    public static CaseFile instance;
    [field: SerializeField] public bool IsOpen { get; private set; }

    [Header("UI")]
    [SerializeField] GameObject bg;

    [Header("Character Info")]
    [SerializeField] TextMeshProUGUI nameText;

    [Header("Answers")]
    [SerializeField] Transform buttonHolder;
    [SerializeField] List<FileButton> fileButtons = new List<FileButton>();
    [SerializeField] AnswerText[] answerTexts;

    [SerializeField] ProfileSO[] profileObjects;

    CharacterProfile[] profiles;
    CharacterProfile selectedProfile;
    public event Action<CharacterProfile> onProfileSelect; // Subscribbers: FileButton
    public event Action<Evidence> onAnswerSelect;

    [Header("Verdict")]
    [SerializeField] TextMeshProUGUI conclusion;
    [SerializeField] Color guiltyColour = Color.red;
    [SerializeField] Color notGuiltyColour = Color.green;
    public AnswerText[] AnswerTexts => answerTexts;

    public FileButton selectedButton;
    Evidence correctAnswer;

    private void Awake()
    {
        instance = this;
        answerTexts = GetComponentsInChildren<AnswerText>(true);

        foreach (Transform child in buttonHolder)
        {
            if (child.TryGetComponent<FileButton>(out FileButton button))
            {
                fileButtons.Add(button);
            }
        }
    }

    private void Start()
    {
        CloseCaseFile();
        conclusion.gameObject.SetActive(false);
        HideQuestions();
    }

    #region Toggle
    public void OpenCaseFile()
    {
        GameManager.instance.SwitchState(InteractionState.CaseFile);
        GameManager.instance.ShowMouse();
        IsOpen = true;

        bg.SetActive(true);

        profiles = GetComponentsInChildren<CharacterProfile>();
    }

    public void CloseCaseFile()
    {
        GameManager.instance.SwitchState(InteractionState.None);
        GameManager.instance.HideMouse();
        IsOpen = false;

        bg.SetActive(false);
    }
    #endregion

    #region Profile Info
    public void ShowProfileInfo(CharacterProfile profile)
    {
        //SetNameText(profile.profile._name);
    }

    public void SetNameText(string _name)
    {
        nameText.text = _name;
    }

    #endregion

    #region Conclusion
    public void DeclareGuilty(bool isGuilty)
    {
        conclusion.gameObject.SetActive(true);
        if (isGuilty)
        {
            conclusion.text = "Guilty";
            conclusion.color = guiltyColour;
        }
        else
        {
            conclusion.text = "Not Guilty";
            conclusion.color = notGuiltyColour;
        }
    }
    #endregion

    #region Profile Functions
    public void SelectProfile(CharacterProfile profile)
    {
        selectedProfile = profile; // Set the selected profile
        nameText.text = profile.ProfileSO._name; // Update the info screen
        ShowButtons();

        foreach (FileButton fileButton in fileButtons)
        {
            fileButton.SetProfile(profile);
        }
    }

    void HideQuestions()
    {
        buttonHolder.gameObject.SetActive(false);
    }

    void ShowButtons()
    {
        buttonHolder.gameObject.SetActive(true);
    }

    public void AnswerQuestion(FileButton selectedButton, Evidence correctAnswer)
    {
        EvidenceDisplay.instance.OpenEvidenceBoard();
        EvidenceDisplay.instance.inCaseFile = true;
        GameManager.instance.SwitchState(InteractionState.EvidenceBoard);
        EvidenceDisplay.instance.onEvidenceClick += OnEvidenceClick;

        // Disable all the buttons
        foreach (FileButton fileButton in fileButtons)
        {
            fileButton.GetComponent<Button>().enabled = false;
        }

        this.correctAnswer = correctAnswer;
        this.selectedButton = selectedButton;
    }

    private void OnEvidenceClick(Evidence evidence)
    {
        Debug.Log("Evidence Clicked From Case File");
        EvidenceDisplay.instance.CloseEvidenceBoard();
        EvidenceDisplay.instance.inCaseFile = false;
        GameManager.instance.SwitchState(InteractionState.CaseFile);
        EvidenceDisplay.instance.onEvidenceClick -= OnEvidenceClick;

        foreach (FileButton fileButton in fileButtons)
        {
            fileButton.GetComponent<Button>().enabled = true;
        }

        if (evidence == selectedButton.Answer)
        {
            selectedButton.SolveAnswer();
        }

        correctAnswer = null;
        selectedButton = null;
    }
    #endregion
}

/*HOW SHOULD THE CASE FILE WORK?
 * STEP 1: OPEN HE CASE FILE
 *     ### Go open the case file 
 *     ### Change the game state to case file 
 * 
 * STEP 2: CLICK ON A CHARACTER PROFILE 
 *     ### Click on the profile
 *     ### Activate the answer buttons 
 *     ### Pass the answers of the profile to the buttons so each knows what it's answer is
 * 
 * STEP 3: CLICK ON AN ANSWER BUTTON
 *     ### Click on the answer button
 *     ### Open the evidence board
 *     ### Select evidence in the case board
 *     ### Close the evidence board
 *     ### Check if the chosen evidence is the same as the button's answer
 *     ### if correct:
 *         ### Disable the button and activate the answer text 
 *     else:
 *         Show a prompt that says the answer was wrong (maybe a hint depending on the profile?)
 *         
 * REQUIREMENTS:
 *      You need to be able to close the evidence board by pressing it's button
 *      The buttons need to automatically update depending on the chosen profile
 */