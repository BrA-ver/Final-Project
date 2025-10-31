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
    [SerializeField] TextMeshProUGUI nameText;

    public Evidence Evidence => evidence;

    public event Action<Evidence> onSlotClick;

    public void Initialize(Evidence newEvidence, EvidenceDisplay display)
    {
        evidence = newEvidence;
        icon.sprite = evidence.sprite;
        this.display = display;
        nameText.text = evidence.Name;
    }

    public void OnClick()
    {
        display.SelectSlot(this);
    }
}
