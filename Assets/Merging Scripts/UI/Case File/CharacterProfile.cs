using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CharacterProfile : MonoBehaviour
{
    CaseFile file;

    [Header("Profile")]
    [SerializeField] ProfileSO profile;

    public bool solvedMotive;
    public bool solvedMeans;
    public bool solvedOpportunity;

    bool isGuilty;

    [Header("UI")]
    [SerializeField] Image picture;

    public UnityEvent OnProfileSolved;


    public bool Complete => solvedMotive && solvedMeans && solvedOpportunity;
    public bool IsGuity => isGuilty;
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
        CheckCompletion();
    }

    public void SolveMeans()
    {
        solvedMeans = true;
        CheckCompletion();
    }

    public void SolveOpportunity()
    {
        solvedOpportunity = true;
        CheckCompletion();
    }

    public void CheckCompletion()
    {
        if (solvedMeans && solvedMotive && solvedOpportunity)
            OnProfileSolved?.Invoke();
    }

    public void SetGuilty(bool isGuilty)
    {
        this.isGuilty = isGuilty;
    }
}


