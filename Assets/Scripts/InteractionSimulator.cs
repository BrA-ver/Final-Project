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

        
        if (caseData != null)
        {
            caseData.InitializeDictionary();
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
        if (caseData == null)
        {
            Debug.LogError("CaseData reference is not set!");
            return;
        }

        CaseData.NPCData npc = caseData.GetNPCData(npcID);

        if (npc != null && !discoveredNPCs.Contains(npcID))
        {
            // Add to discovered NPCs
            discoveredNPCs.Add(npcID);

            
            if (caseboardController != null)
            {
                caseboardController.AddCaseItem(npc.npcPhoto, npc.npcName, npc.npcDescription);
            }

            Debug.Log($"Added case item for NPC: {npc.npcName}");
        }
        else if (npc != null && discoveredNPCs.Contains(npcID))
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

    public List<string> GetDiscoveredNPCs()
    {
        return new List<string>(discoveredNPCs);
    }
}
