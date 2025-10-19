using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CaseFile : MonoBehaviour
{
    public static CaseFile instance;
    [field: SerializeField] public bool IsOpen { get; private set; }

    [Header("UI")]
    [SerializeField] GameObject bg;

    [Header("Character Info")]
    [SerializeField] TextMeshProUGUI nameText;

    [Header("Answers")]
    [SerializeField] GameObject buttonHolder;
    [SerializeField] FileButton[] fileButtons;
    [SerializeField] AnswerText[] answerTexts;

    [SerializeField] ProfileSO[] profileObjects;

    CharacterProfile[] profiles;
    CharacterProfile selectedProfile;
    public event Action<CharacterProfile> onProfileSelect;
    public event Action<Evidence> onAnswerSelect;

    [Header("Verdict")]
    [SerializeField] TextMeshProUGUI conclusion;
    [SerializeField] Color guiltyColour = Color.red;
    [SerializeField] Color notGuiltyColour = Color.green;

    public FileButton[] FileButtons => fileButtons;
    public AnswerText[] AnswerTexts => answerTexts;

    private void Awake()
    {
        instance = this;
        fileButtons = GetComponentsInChildren<FileButton>(true);
        answerTexts = GetComponentsInChildren<AnswerText>(true);
    }

    private void Start()
    {
        CloseCaseFile();
        conclusion.gameObject.SetActive(false);
        HideQuestions();
    }

    #region Answering Questions
    public void ClickButton()
    {

    }
    #endregion

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

        //UnsubscribeProfiles();
        ResetPage();
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

    public void UnsubscribeProfiles()
    {
        if (profiles.Length <= 0 || profiles == null) return;
        foreach (CharacterProfile profile in profiles)
        {
            //profile.UnSubscribeToButtonSolved();
        }
    }

    void ResetPage()
    {

    }

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


    public void SelectProfile(CharacterProfile profile)
    {
        selectedProfile = profile; // Set the selected profile
        nameText.text = profile.ProfileSO._name; // Update the info screen
        ShowButtons();

        onProfileSelect?.Invoke(selectedProfile);
    }

    void HideQuestions()
    {
        buttonHolder.SetActive(false);
    }

    void ShowButtons()
    {
        buttonHolder.SetActive(true);
    }

    public void PickAnswer()
    {
        EvidenceDisplay.instance.OpenEvidenceBoard();
        GameManager.instance.SwitchState(InteractionState.EvidenceBoard);
        EvidenceDisplay.instance.onEvidenceClick += OnEvidenceClick;

        // Disable all the buttons
    }

    private void OnEvidenceClick(Evidence evidence)
    {
        onAnswerSelect?.Invoke(evidence);
        EvidenceDisplay.instance.onEvidenceClick -= OnEvidenceClick;
    }
}
