using System;
using UnityEngine;
using UnityEngine.UI;

public class FileButton : MonoBehaviour
{
    CaseFile file;
    CharacterProfile profile;

    private void Start()
    {
        file = CaseFile.instance;
        file.onProfileSelect += OnProfileSelect;
    }

    private void OnDestroy()
    {
        file.onProfileSelect -= OnProfileSelect;
    }

    private void OnProfileSelect(CharacterProfile profile)
    {
        this.profile = profile;
    }
}
public enum AnswerType { Motive, Means, Opportunity }