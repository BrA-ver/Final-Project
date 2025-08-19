using UnityEngine;

[CreateAssetMenu(fileName = "Evidence", menuName = "Scriptable Objects/Evidence")]
public class Evidence : ScriptableObject
{
    public string Name;
    [TextArea(2, 3)] public string Description;
}
