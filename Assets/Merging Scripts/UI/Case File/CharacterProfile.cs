using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProfile : MonoBehaviour
{
    CaseFile file;

    [Header("Profile")]
    [SerializeField] ProfileSO profile;

    public bool solvedMotive;
    public bool solvedMeans;
    public bool solvedOpportunity;

    [Header("UI")]
    [SerializeField] Image picture;


    public ProfileSO ProfileSO => profile;

    private void Start()
    {
        file = CaseFile.instance;

        picture.color = Color.white;
        picture.sprite = profile.picture;
    }

    public void SetProfile(ProfileSO profile)
    {
        this.profile = profile;
    }

    public void ClickProfile()
    {
        file.SelectProfile(this);
    }

    public void SolveMotive()
    {
        solvedMotive = true;
    }

    public void SolveMeans()
    {
        solvedMeans = true;
    }

    public void SolveOpportunity()
    {
        solvedOpportunity = true;
    }
}


