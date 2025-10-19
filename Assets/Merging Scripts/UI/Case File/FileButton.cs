using System;
using UnityEngine;
using UnityEngine.UI;

public class FileButton : MonoBehaviour
{
    CaseFile file;
    [SerializeField] AnswerType type;
    [SerializeField] Evidence answer;
    Button button;

    private void Start()
    {
        file = CaseFile.instance;
        file.onProfileSelect += OnProfileSelect;

        button = GetComponent<Button>();
    }

    private void OnDisable()
    {
        if (file)
            file.onProfileSelect -= OnProfileSelect;
    }

    public void OnClick()
    {
        file.PickAnswer();
        file.onAnswerSelect += OnAnswerSelect;
    }

    private void OnProfileSelect(CharacterProfile profile)
    {
        if (profile.solvedMotive || profile.solvedMeans || profile.solvedOpp)
        {
            ShowAnswer(profile);
        }
    }

    void ShowAnswer(CharacterProfile profile)
    {
        switch (type)
        {
            case AnswerType.Motive:
                if (profile.solvedMotive)
                {
                    // Show Answer
                }
                break;
        }
    }

    private void OnAnswerSelect(Evidence answer)
    {
        if (answer != this.answer)
        {
            Debug.Log("Wrong");
        }
        else
        {
            Debug.Log("Correct");
        }
    }

    //[SerializeField] Evidence requiredEvidence;

    //[Header("Type")]
    //public AnswerType type;


    //public event Action<AnswerType> onButtonSolved;

    //private void OnDisable()
    //{
    //    EvidenceDisplay.instance.onEvidenceClick -= OnEvidenceClick;
    //}

    //public void OpenEvidenceBoard() // Called when the button is clicked
    //{
    //    EvidenceDisplay.instance.OpenEvidenceBoard();
    //    GameManager.instance.SwitchState(InteractionState.EvidenceBoard);
    //    EvidenceDisplay.instance.onEvidenceClick += OnEvidenceClick; // Subribes the button to the evidence click
    //}

    //private void OnEvidenceClick(Evidence evidence)
    //{
    //    if (evidence.Name != requiredEvidence.Name)
    //        Debug.Log("Wrong");
    //    else
    //    {
    //        Debug.Log("Correct");
    //        onButtonSolved?.Invoke(type);
    //    }
    //}

}
public enum AnswerType { Motive, Means, Opportunity }