using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class FileButton : MonoBehaviour
{
    CaseFile file;
    CharacterProfile profile;
    [SerializeField] Button button;

    [SerializeField] AnswerType type;
    [SerializeField] Evidence answer;
    [SerializeField] TextMeshProUGUI answerText;
    [SerializeField] TextMeshProUGUI buttonText;

    public Evidence Answer => answer;

    Dictionary<AnswerType, string> typeText;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void Initialize()
    {
        file = CaseFile.instance;

        typeText = new Dictionary<AnswerType, string>
        {
            {AnswerType.Motive, "Motive?" },
            {AnswerType.Means, "Means?" },
            {AnswerType.Opportunity, "Opportunity?" }
        };
    }

    public void SetButtonEnabled(bool enabled)
    {
        button.enabled = enabled;
    }


    public void SetProfile(CharacterProfile profile)
    {
        this.profile = profile;


        switch (type)
        {
            case AnswerType.Motive:
                answer = profile.ProfileSO.motive;

                // If the anser is solved, show the answer
                ToggleAnserText(profile.solvedMotive, profile.ProfileSO.Motive);
                break;
            case AnswerType.Means:
                answer = profile.ProfileSO.means;

                ToggleAnserText(profile.solvedMeans, profile.ProfileSO.Means);
                break;
            case AnswerType.Opportunity:
                answer = profile.ProfileSO.opportunity;

                ToggleAnserText(profile.solvedOpportunity, profile.ProfileSO.Opportunity);
                break;
        }
    }

    private void ToggleAnserText(bool solved, string text)
    {
        Debug.Log($"Solved: {text}");
        if (!solved)
        {
            SetButtonEnabled(true);
            answerText.text = string.Empty;

            Debug.Log($"Type text is null: {typeText == null}");
            buttonText.text = typeText[type];
            
            return;
        }

        SetButtonEnabled(false);
        buttonText.text = string.Empty;
        answerText.text = text;
    }

    public void AnswerQuestion()
    {
        file.AnswerQuestion(this, answer);
    }

    public void SolveAnswer()
    {
        switch (type)
        {
            case AnswerType.Motive:
                profile.SolveMotive();
                ToggleAnserText(true, profile.ProfileSO.Motive);
                button.enabled = false;
                break;
            case AnswerType.Means:
                profile.SolveMeans();
                ToggleAnserText(true, profile.ProfileSO.Means);
                button.enabled = false;
                break;
            case AnswerType.Opportunity:
                profile.SolveOpportunity();
                ToggleAnserText(true, profile.ProfileSO.Opportunity);
                button.enabled = false;
                break;
        }

        // Show button answer
        Debug.Log("Correct Answer");
    }
}
public enum AnswerType { Motive, Means, Opportunity }