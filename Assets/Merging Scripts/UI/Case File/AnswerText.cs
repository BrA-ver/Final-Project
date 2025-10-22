using UnityEngine;
using TMPro;
using System;

public class AnswerText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI answerText;

    

    public void SetText(string text)
    {
        answerText.text = text;
    }
}
