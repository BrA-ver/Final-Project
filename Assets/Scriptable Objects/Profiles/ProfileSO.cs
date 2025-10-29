using UnityEngine;

[CreateAssetMenu(fileName = "ProfileSO", menuName = "Scriptable Objects/ProfileSO")]
public class ProfileSO : ScriptableObject
{
    [Header("Info")]
    public string _name;
    public string occupation;
    public Sprite picture;

    [Header("Dialogue")]
    [field: SerializeField] public Dialogue MainDialogue { get; private set; }// The dialogue that will lead to all the other dialogues

    [Header("Evidence")]
    [SerializeField] EvidenceResponce[] evidenceResponces;

    [Header("Case Data")]
    public Evidence motive;
    public Evidence means;
    public Evidence opportunity;

    [Header("Answers")]
    [SerializeField] [TextArea(2, 5)] string _motive;
    [SerializeField] [TextArea(2, 5)] string _means;
    [SerializeField] [TextArea(2, 5)] string _opportunity;

    public string Motive => _motive;
    public string Means => _means;
    public string Opportunity => _opportunity;
}
