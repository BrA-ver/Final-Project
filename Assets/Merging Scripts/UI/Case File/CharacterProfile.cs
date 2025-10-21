using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProfile : MonoBehaviour
{
    Button button;
    CaseFile file;


    [field: SerializeField] public ProfileSO ProfileSO { get; private set; }

    [Header("UI")]
    [SerializeField] Image characterImage;

    [Header("Case Data")]
    public bool solvedMotive, solvedMeans, solvedOpp;

    private void Start()
    {
        file = CaseFile.instance;
        button = GetComponent<Button>();

        if (ProfileSO == null || ProfileSO.picture == null)
        {
            characterImage.color = Color.black;
        }
        else
        {
            characterImage.color = Color.white;
            characterImage.sprite = ProfileSO.picture;
        }
    }

    public void SelectProfile()
    {
        file.SelectProfile(this);
    }

    //CaseFile file;

    //public ProfileSO profile;

    //FileButton[] fileButtons;
    //AnswerText[] answerTexts;

    

    //[SerializeField] bool subribed = false;

    //private void Start()
    //{
    //    file = CaseFile.instance;
    //    fileButtons = file.FileButtons;
    //    answerTexts = file.AnswerTexts;
    //}

    //private void OnButtonSolved(AnswerType type)
    //{
    //    foreach (FileButton button in fileButtons)
    //    {
    //        if (button.type == type)
    //        {
    //            button.gameObject.SetActive(false);
    //        }
    //    }

    //    foreach (AnswerText text in answerTexts)
    //    {
    //        if (text.type == type)
    //        {
    //            text.gameObject.SetActive(true);
    //            SetProfileText(type, text);
    //        }
    //    }
    //}

    //void SetProfileText(AnswerType answerType, AnswerText text)
    //{
    //    switch (answerType)
    //    {
    //        case AnswerType.Motive:
    //            text.SetText(profile.Motive);
    //            solvedMotive = true;
    //            break;
    //        case AnswerType.Means:
    //            text.SetText(profile.Means);
    //            solvedMeans = true;
    //            break;
    //        case AnswerType.Opportunity:
    //            text.SetText(profile.Opportunity);
    //            solvedOpp = true;
    //            break;
    //    }
    //}

    //public void SelectProfile() // Called when the profile is clicked
    //{
    //    file.ShowProfileInfo(this);
    //    file.UnsubscribeProfiles();
    //    SubscribeToButtonSolved();

    //    foreach (FileButton button in fileButtons)
    //    {
    //        //Debug.Log("Start Loop");
    //        switch (button.type)
    //        {
    //            case AnswerType.Motive:
    //                if (solvedMotive)
    //                {
    //                    //Debug.Log("Solved Motive");
    //                    // Turn off the button
    //                    button.gameObject.SetActive(false);

    //                    // Turn on the solved text
    //                    ToggleAnswerText(AnswerType.Motive, true);
    //                }
    //                else
    //                {
    //                    button.gameObject.SetActive(true);
    //                    ToggleAnswerText(AnswerType.Motive, false);
    //                }
    //                break;

    //            case AnswerType.Means:
    //                if (solvedMeans)
    //                {
    //                    //Debug.Log("Solved Motive");
    //                    // Turn off the button
    //                    button.gameObject.SetActive(false);

    //                    // Turn on the solved text
    //                    ToggleAnswerText(AnswerType.Means, true);
    //                }
    //                else
    //                {
    //                    button.gameObject.SetActive(true);
    //                    ToggleAnswerText(AnswerType.Means, false);
    //                }
    //                break;

    //            case AnswerType.Opportunity:
    //                if (solvedOpp)
    //                {
    //                    //Debug.Log("Solved Motive");
    //                    // Turn off the button
    //                    button.gameObject.SetActive(false);

    //                    // Turn on the solved text
    //                    ToggleAnswerText(AnswerType.Opportunity, true);
    //                }
    //                else
    //                {
    //                    button.gameObject.SetActive(true);
    //                    ToggleAnswerText(AnswerType.Opportunity, false);
    //                }
    //                break;
    //        }
    //        //Debug.Log("loop");
    //    }
    //}

    //public void SubscribeToButtonSolved()
    //{
    //    Debug.Log(name + " scubscribed");
    //    subribed = true;
    //    foreach (FileButton button in fileButtons)
    //    {
    //        button.onButtonSolved += OnButtonSolved;
    //    }
    //}

    //public void UnSubscribeToButtonSolved() // Called when another profile is selected
    //{
    //    Debug.Log(name + " unscubscribed");
    //    subribed = false;
    //    foreach (FileButton button in fileButtons)
    //    {
    //        button.onButtonSolved -= OnButtonSolved;
    //    }
    //}

    //void ToggleAnswerText(AnswerType type, bool turnOn)
    //{
    //    foreach (AnswerText text in answerTexts)
    //    {
    //        if (text.type == type)
    //        {
    //            text.gameObject.SetActive(turnOn);
    //        }
    //    }
    //}

    //void UpdateProfileData()
    //{

    //}
}
