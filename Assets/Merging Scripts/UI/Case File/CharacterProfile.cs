using UnityEngine;

public class CharacterProfile : MonoBehaviour
{
    CaseFile file;

    [SerializeField] ProfileSO profile;

    private void Start()
    {
        file = CaseFile.instance;
    }

    public void SelectProfile() // Called when the profile is clicked
    {
        file.SetNameText(profile._name);
    }
}
