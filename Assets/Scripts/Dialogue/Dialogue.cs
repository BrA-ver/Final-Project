using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class Dialogue : ScriptableObject
{
    public string _name;
    public string Speaker = string.Empty;
    public string[] lines;

    public DialogueChoice[] choices;

    [Header("Evidence")]
    public Evidence evidence;
}

[System.Serializable]
public class DialogueChoice
{
    public string text;
    public Dialogue targetDialogue;
}