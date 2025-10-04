using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaseFile : MonoBehaviour
{
    public static CaseFile instance;
    [field: SerializeField] public bool IsOpen { get; private set; }

    [Header("UI")]
    [SerializeField] GameObject bg;
    [SerializeField] TextMeshProUGUI nameText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CloseCaseFile();
    }

    #region Toggle
    public void OpenCaseFile()
    {
        GameManager.instance.SwitchState(InteractionState.CaseFile);
        GameManager.instance.ShowMouse();
        IsOpen = true;

        bg.SetActive(true);
    }

    public void CloseCaseFile()
    {
        GameManager.instance.SwitchState(InteractionState.None);
        GameManager.instance.HideMouse();
        IsOpen = false;

        bg.SetActive(false);
    }
    #endregion

    public void SetNameText(string _name)
    {
        nameText.text = _name;
    }
}
