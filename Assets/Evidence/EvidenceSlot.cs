using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvidenceSlot : MonoBehaviour
{
    [SerializeField] Evidence evidence;

    [SerializeField] Image icon;
    [SerializeField] GameObject nameHolder;
    [SerializeField] TextMeshProUGUI nameText;

    [Header("Selection")]
    [SerializeField] GameObject highlight;

    public Evidence Evidence => evidence;

    InputHandler handler;

    private void Awake()
    {
        handler = FindObjectOfType<InputHandler>();
    }

    public void SetEvidence(Evidence newEvidence)
    {
        evidence = newEvidence;
        UpdateSlot();
    }

    private void UpdateSlot()
    {
        icon.sprite = evidence.sprite;
    }

    // Selection
    public void Select()
    {
        highlight.SetActive(true);
        nameHolder.SetActive(true);
        nameText.text = evidence.Name;

        if (EvidenceDisplay.instance.IsInterogating)
            handler.onSumbit += OnSubmit;

    }

    public void Deselect()
    {
        highlight.SetActive(false);
        nameHolder.SetActive(false);
        nameText.text = string.Empty;

        if (EvidenceDisplay.instance.IsInterogating)
            handler.onSumbit -= OnSubmit;
    }

    void OnSubmit()
    {
        EvidenceManager.Instance.PresentEvidence(evidence);
        EvidenceDisplay.instance.CloseEvidenceBoard();
    }
}
