using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaseFile : MonoBehaviour
{
    public static CaseFile instance;
    [field: SerializeField] public bool IsOpen { get; private set; }

    [Header("UI")]
    [SerializeField] GameObject bg;
    [SerializeField] TextMeshProUGUI nameText;

    [SerializeField] FileButton[] fileButtons;
    [SerializeField] AnswerText[] answerTexts;

    [SerializeField] ProfileSO[] profileObjects;

    CharacterProfile[] profiles;

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

        //UnsubscribeProfiles();
        ResetPage();
    }
    #endregion

    #region Profile Info
    public void ShowProfileInfo(CharacterProfile profile)
    {
        SetNameText(profile.profile._name);
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
            profile.UnSubscribeToButtonSolved();
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

}
