using UnityEngine;

[CreateAssetMenu(fileName = "ProfileSO", menuName = "Scriptable Objects/ProfileSO")]
public class ProfileSO : ScriptableObject
{
    public string _name;
    public Evidence motive;
    public Evidence means;
    public Evidence opportunity;
}
