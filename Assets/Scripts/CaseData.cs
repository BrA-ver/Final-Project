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
    public Dictionary<string, NPCData> npcDictionary = new Dictionary<string, NPCData>();

    private void OnEnable()
    {
        
        npcDictionary.Clear();
        foreach (var npc in allNPCs)
        {
            npcDictionary[npc.npcID] = npc;
        }
    }
}