using System.Collections.Generic;
using UnityEngine;

public class InteractionSimulator : MonoBehaviour
{
    [Header("Case Data")]
    public CaseData caseData;

    [Header("Key Bindings")]
    public KeyCode[] interactionKeys = new KeyCode[]
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6
    };

    [Header("NPC IDs to Simulate")]
    public string[] npcIDsToSimulate;

    private CaseBoardController caseboardController;
    private List<string> discoveredNPCs = new List<string>();

    void Start()
    {
        caseboardController = FindObjectOfType<CaseBoardController>();

        if (caseboardController == null)
        {
            Debug.LogError("CaseboardController not found in the scene!");
        }
    }

    void Update()
    {
        // Check for interaction keys
        for (int i = 0; i < Mathf.Min(interactionKeys.Length, npcIDsToSimulate.Length); i++)
        {
            if (Input.GetKeyDown(interactionKeys[i]))
            {
                string npcID = npcIDsToSimulate[i];
                SimulateInteraction(npcID);
            }
        }
    }

    public void SimulateInteraction(string npcID)
    {
        if (caseData.npcDictionary.ContainsKey(npcID) && !discoveredNPCs.Contains(npcID))
        {
            CaseData.NPCData npc = caseData.npcDictionary[npcID];

            // Add to discovered NPCs
            discoveredNPCs.Add(npcID);

            
            if (caseboardController != null)
            {
                caseboardController.AddCaseItem(npc.npcPhoto, npc.npcName, npc.npcDescription);
            }

            Debug.Log($"Added case item for NPC: {npc.npcName}");
        }
        else if (discoveredNPCs.Contains(npcID))
        {
            Debug.Log($"Already discovered NPC with ID: {npcID}");
        }
        else
        {
            Debug.LogWarning($"NPC with ID {npcID} not found in CaseData!");
        }
    }

    public void ResetDiscoveredNPCs()
    {
        discoveredNPCs.Clear();
        if (caseboardController != null)
        {
            caseboardController.ClearAllCaseItems();
        }
    }
}
