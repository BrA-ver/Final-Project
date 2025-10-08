using UnityEngine;

[CreateAssetMenu(fileName = "ProfileSO", menuName = "Scriptable Objects/ProfileSO")]
public class ProfileSO : ScriptableObject
{
    [Header("test")]
    public string _name;
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
