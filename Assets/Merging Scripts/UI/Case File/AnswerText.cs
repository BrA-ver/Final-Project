using UnityEngine;
using TMPro;

public class AnswerText : MonoBehaviour
{
    public AnswerType type;
    [SerializeField] TextMeshProUGUI answerText;

    public void SetText(string text)
    {
        answerText.text = text;
    }
}
