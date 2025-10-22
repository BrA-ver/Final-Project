using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProfile : MonoBehaviour
{
    CaseFile file;

    [Header("Profile")]
    [SerializeField] ProfileSO profile;

    public ProfileSO ProfileSO => profile;

    private void Start()
    {
        file = CaseFile.instance;
    }

    public void ClickProfile()
    {
        file.SelectProfile(this);
    }
}


