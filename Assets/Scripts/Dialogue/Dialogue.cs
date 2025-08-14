using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    public string Speaker = string.Empty;
    [TextArea(1, 2)] public string Line = string.Empty;
}
