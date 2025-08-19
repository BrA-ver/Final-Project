using UnityEngine;
using System;
using System.Collections.Generic;
public class EvidenceManager : MonoBehaviour
{
    public static EvidenceManager Instance;

    InputHandler input;

    [SerializeField] Evidence evidence;
    [SerializeField] List<Evidence> allEvidence = new List<Evidence>();

    public event Action<Evidence> onPresentEvidence;

    private void Awake()
    {
        Instance = this;
        input = FindFirstObjectByType<InputHandler>();
        AddEvidence(evidence);
    }

    private void OnEnable()
    {
        input.onPresentEvidence += OnPresentEvidence;
    }

    private void OnDisable()
    {
        input.onPresentEvidence -= OnPresentEvidence;
    }

    private void OnPresentEvidence()
    {
        onPresentEvidence?.Invoke(evidence);
    }

    public void AddEvidence(Evidence evidence)
    {
        if (allEvidence.Contains(evidence)) return;
        Debug.Log($"Added {evidence.Name} to evidence list");
        allEvidence.Add(evidence);
    }
}
