using UnityEngine;
using System.Collections.Generic;

public class EvidenceDisplay : MonoBehaviour
{
    public static EvidenceDisplay instance;

    [SerializeField] EvidenceSlot slotPrefab;
    [SerializeField] RectTransform evidenceHolder;
    [SerializeField] GameObject parent;

    [SerializeField] int maxSlots = 12;
    List<EvidenceSlot> activeSlots = new List<EvidenceSlot>();

    List<Evidence> evidences;

    // Selecting Slots
    [SerializeField] int index = 0;
    EvidenceSlot selectedSlot;

    public bool IsOpen;

    private void Awake()
    {
        instance = this;
    }

    public void OpenEvidenceBoard()
    {
        IsOpen = true;
        parent.SetActive(true);
        evidences = EvidenceManager.Instance.AllEvidence;
        ShowEvidence();
        index = 0;

        SelectSlot();
    }

    void ShowEvidence()
    {
        // Spawn evidence slot for each evidence
        foreach(Evidence evidence in evidences)
        {
            EvidenceSlot slot = Instantiate(slotPrefab, evidenceHolder);

            // Assign evidence to the slot
            slot.SetEvidence(evidence);

            // Add the slot to the active slots list
            activeSlots.Add(slot);
        }
    }

    void SelectSlot()
    {
        if (selectedSlot)
            selectedSlot.Deselect();

        selectedSlot = activeSlots[index];
        selectedSlot.Select();
    }

    public void ToggleSlot(float slot)
    {
        if (slot > 0.1)
        {
            index++;
            if (index >= activeSlots.Count)
                index = 0;
        }
        else if (slot < -0.1)
        {
            index--;
            if (index < 0)
                index = activeSlots.Count - 1;
        }

        SelectSlot();
    }
}
