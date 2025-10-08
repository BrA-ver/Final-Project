using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EvidenceSlot : MonoBehaviour
{
    EvidenceDisplay display;
    [Header("Evidence")]
    [SerializeField] Evidence evidence;

    [Header("Button Look")]
    [SerializeField] Image icon;
    [SerializeField] GameObject nameHolder;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject highlight;

    public Evidence Evidence => evidence;

    public void Initialize(Evidence newEvidence, EvidenceDisplay display)
    {
        evidence = newEvidence;
        icon.sprite = evidence.sprite;
        this.display = display;
    }

    public void OnClick()
    {
        display.ClickEvidence(evidence);
    }

    #region Button Events

    public void PresentEvidence() // Called when the slot button is pressed
    {
        Debug.Log("Clicked Slot");
        EvidenceManager.Instance.PresentEvidence(evidence);
    }

    public void Select() // Called when the slot is selected, but not pressed
    {
        highlight.SetActive(true);
        nameHolder.SetActive(true);
        nameText.text = evidence.Name;

        EvidenceDisplay.instance.ShowDescription(this);
    }

    

    public void Deselect() // Called When the slot is deselected
    {
        highlight.SetActive(false);
        nameHolder.SetActive(false);
        nameText.text = string.Empty;
    }
    #endregion

    private void OnDestroy()
    {
        Deselect();
    }
}
