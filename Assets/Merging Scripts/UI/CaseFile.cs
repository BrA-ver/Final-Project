using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Collections;

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

    [Header("Character Profiles")]
    [SerializeField] CharacterProfile profilePrefab;
    [SerializeField] NPC[] suspects;
    [SerializeField] ProfileSO[] profileObjects;
    [SerializeField] GameObject profileHolder;

    List<CharacterProfile> activeProfiles = new List<CharacterProfile>();

    CharacterProfile[] profiles;
    CharacterProfile selectedProfile;
    public event Action<CharacterProfile> onProfileSelect; // Subscribbers: FileButton
    public event Action<Evidence> onAnswerSelect;

    [Header("Verdict")]
    [SerializeField] GameObject verdictHolder;
    [SerializeField] TextMeshProUGUI verdictText;
    [SerializeField] Image verdictCircle;
    [SerializeField] float fillTime = .5f;
    [SerializeField] Color guiltyColour = Color.red;
    [SerializeField] Color notGuiltyColour = Color.green;
    [SerializeField] Button guiltyButton, notGuiltyButton;
    public event Action onVerdictMade;

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
        verdictText.gameObject.SetActive(false);
        HideQuestions();

        guiltyButton.onClick.AddListener(() => {
            DeclareVerdict(true);
        });
        notGuiltyButton.onClick.AddListener(() => {
            DeclareVerdict(false);
        });

        NPC[] allNpcs = FindObjectsByType<NPC>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        suspects = Array.FindAll(allNpcs, i => i.isSuspect);
        profileObjects = new ProfileSO[suspects.Length];
        for (int i = 0; i < suspects.Length; i++)
        {
            profileObjects[i] = suspects[i].Profile;
        }
    }

    #region Toggle
    public void OpenCaseFile()
    {
        GameManager.instance.SwitchState(InteractionState.CaseFile);
        GameManager.instance.ShowMouse();
        IsOpen = true;

        bg.SetActive(true);

        profiles = GetComponentsInChildren<CharacterProfile>();

        bool isListEmpty = activeProfiles.Count <= 0;
        if (isListEmpty)
        {
            // Spawn the character profiles, if they are not spawned
            for (int i = 0; i < suspects.Length; i++)
            {
                CharacterProfile newProfile = Instantiate(profilePrefab, profileHolder.transform);
                newProfile.SetProfile(profileObjects[i]);

                activeProfiles.Add(newProfile);
            }
        }
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

    #region Verdict

    void ShowVerdictBlock()
    {
        verdictHolder.SetActive(true);
    }

    void HideVerdict()
    {
        verdictHolder.SetActive(false);

        verdictText.text = "";
        verdictCircle.fillAmount = 0f;
    }

    public void DeclareVerdict(bool isGuilty)
    {
        verdictText.gameObject.SetActive(true);
        selectedProfile.SetGuilty(isGuilty);
        if (isGuilty)
        {
            verdictText.text = "Guilty";
            verdictText.color = guiltyColour;
            verdictCircle.color = guiltyColour;
        }
        else
        {
            verdictText.text = "Not Guilty";
            verdictText.color = notGuiltyColour;
            verdictCircle.color = notGuiltyColour;
        }
        StartCoroutine(FillCircleRoutine(fillTime));
        onVerdictMade?.Invoke();
    }

    IEnumerator FillCircleRoutine(float fillTIme)
    {
        float timePassed = 0f;
        while (timePassed < fillTIme)
        {
            timePassed += Time.deltaTime;

            float fill = Mathf.Lerp(0f, 1f, timePassed / fillTIme);
            verdictCircle.fillAmount = fill;
            yield return null;
        }
    }

    #endregion

    #region Profile Functions
    public void SelectProfile(CharacterProfile profile)
    {
        if (selectedProfile != null)
            selectedProfile.OnProfileSolved.RemoveListener(ShowVerdictBlock);

        selectedProfile = profile; // Set the selected profile
        selectedProfile.OnProfileSolved.AddListener(ShowVerdictBlock);
        if (!selectedProfile.Complete)
        {
            HideVerdict();
        }
        else
        {
            if (selectedProfile.IsGuity)
            {
                verdictText.text = "Guilty";
                verdictText.color = guiltyColour;
                verdictCircle.color = guiltyColour;
                verdictCircle.fillAmount = 1f;
            }
            else
            {
                verdictText.text = "Not Guilty";
                verdictText.color = notGuiltyColour;
                verdictCircle.color = notGuiltyColour;
                verdictCircle.fillAmount = 0f;
            }
        }

        nameText.text = profile.ProfileSO._name; // Update the info screen
        ShowButtons();

        foreach (FileButton fileButton in fileButtons)
        {
            fileButton.Initialize();
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
        //foreach (FileButton fileButton in fileButtons)
        //{
        //    fileButton.GetComponent<Button>().enabled = false;
        //}

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

        //foreach (FileButton fileButton in fileButtons)
        //{
        //    fileButton.GetComponent<Button>().enabled = true;
        //}

        if (evidence.Name == selectedButton.Answer.Name)
        {
            Debug.Log($"{evidence.Name} is the same as {selectedButton.Answer.Name}");
            selectedButton.SolveAnswer();
            Debug.Log("go");
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