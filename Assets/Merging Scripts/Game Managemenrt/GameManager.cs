using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [field: SerializeField] public InteractionState CurrentState;
    [field: SerializeField] public InteractionState PreviousState;

    [SerializeField] GameObject selected;

    public bool IgnoreMouseInput = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); }

    }

    private void Update()
    {
        GetSelected();
    }

    public void GetSelected()
    {
        selected = EventSystem.current.currentSelectedGameObject;
    }

    public void SwitchState(InteractionState newState)
    {
        PreviousState = CurrentState;
        CurrentState = newState;
    }

    public void HideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}

public enum InteractionState { None, ActionScreen, EvidenceBoard, Dialogue, CaseFile}