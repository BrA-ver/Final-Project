using System;
using UnityEngine;
using UnityEditor;

public class FileButton : MonoBehaviour
{
    [SerializeField] Evidence requiredEvidence;

    [Header("Type")]
    public AnswerType type;
    

    public event Action<AnswerType> onButtonSolved;

    private void OnDisable()
    {
        EvidenceDisplay.instance.onEvidenceClick -= OnEvidenceClick;
    }

    public void OpenEvidenceBoard()
    {
        EvidenceDisplay.instance.OpenEvidenceBoard();
        EvidenceDisplay.instance.onEvidenceClick += OnEvidenceClick;
    }

    private void OnEvidenceClick(Evidence evidence)
    {
        if (evidence.Name != requiredEvidence.Name)
            Debug.Log("Wrong");
        else
        {
            Debug.Log("Correct");
            onButtonSolved?.Invoke(type);
        }
    }
    
}
public enum AnswerType { Motive, Means, Opportunity }