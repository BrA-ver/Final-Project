using UnityEngine;

[CreateAssetMenu(fileName = "Cutscene", menuName = "Scriptable Objects/Cutscene")]
public class Cutscene : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Sprite[] images { get; private set; }
}
