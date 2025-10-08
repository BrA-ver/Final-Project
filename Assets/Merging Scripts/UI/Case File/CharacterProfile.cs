using System;
using UnityEngine;

public class CharacterProfile : MonoBehaviour
{
    CaseFile file;

    public ProfileSO profile;

    FileButton[] fileButtons;
    AnswerText[] answerTexts;

    public bool solvedMotive, solvedMeans, solvedOpp;

    private void Start()
    {
        file = CaseFile.instance;
        fileButtons = FindObjectsByType<FileButton>(FindObjectsSortMode.None);
        answerTexts = FindObjectsByType<AnswerText>(FindObjectsInactive.Include,FindObjectsSortMode.None);

        foreach (FileButton button in fileButtons)
        {
            button.onButtonSolved += OnButtonSolved;
        }
    }

    private void OnButtonSolved(AnswerType type)
    {
        foreach (FileButton button in fileButtons)
        {
            if (button.type == type)
            {
                button.gameObject.SetActive(false);
            }
        }

        foreach (AnswerText text in answerTexts)
        {
            if (text.type == type)
            {
                text.gameObject.SetActive(true);
                SetProfileText(type, text);
            }
        }
    }

    void SetProfileText(AnswerType answerType, AnswerText text)
    {
        switch (answerType)
        {
            case AnswerType.Motive:
                text.SetText(profile.Motive);
                solvedMotive = true;
                break;
            case AnswerType.Means:
                text.SetText(profile.Means);
                solvedMeans = true;
                break;
            case AnswerType.Opportunity:
                text.SetText(profile.Opportunity);
                solvedOpp = true;
                break;
        }
    }

    public void SelectProfile() // Called when the profile is clicked
    {
        file.ShowProfileInfo(this);

        
        foreach (FileButton button in fileButtons)
        {
            Debug.Log("Start Loop");
            switch (button.type)
            {
                case AnswerType.Motive:
                    if (solvedMotive)
                    {
                        // Turn off the button
                        button.gameObject.SetActive(false);

                        // Turn on the solved text
                    }
                    else button.gameObject.SetActive(true);
                    break;

                case AnswerType.Means:
                    if (solvedMeans) button.gameObject.SetActive(false);
                    else button.gameObject.SetActive(true);
                    break;

                case AnswerType.Opportunity:
                    if (solvedOpp) button.gameObject.SetActive(false);
                    else button.gameObject.SetActive(true);
                    break;
            }
            Debug.Log("loop");
        }
    }

    void UpdateProfileData()
    {

    }
}
