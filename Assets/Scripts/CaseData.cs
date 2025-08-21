using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CaseData", menuName = "Case System/Case Data")]
public class CaseData : ScriptableObject
{
    [System.Serializable]
    public class NPCData
    {
        public string npcID;
        public string npcName;
        public Sprite npcPhoto;
        [TextArea] public string npcDescription;
    }

    public NPCData[] allNPCs;

    private Dictionary<string, NPCData> _npcDictionary;

    public void InitializeDictionary()
    {
        _npcDictionary = new Dictionary<string, NPCData>();
        foreach (var npc in allNPCs)
        {
            _npcDictionary[npc.npcID] = npc;
        }
    }

    public NPCData GetNPCData(string npcID)
    {
        if (_npcDictionary == null)
        {
            InitializeDictionary();
        }

        if (_npcDictionary.ContainsKey(npcID))
        {
            return _npcDictionary[npcID];
        }

        Debug.LogWarning($"NPC with ID {npcID} not found in CaseData!");
        return null;
    }
}