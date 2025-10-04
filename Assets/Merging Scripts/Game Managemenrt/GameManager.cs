using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [field: SerializeField] public bool IsInteracting { get; private set; }
    [field: SerializeField] public InteractionState CurrentState;

    [SerializeField] GameObject selected;

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

    public void StartInteracting()
    {
        IsInteracting = true;
    }

    public void StopInteracting()
    {
        IsInteracting = false;
    }

    public void GetSelected()
    {
        selected = EventSystem.current.currentSelectedGameObject;
    }

    public void SwitchState(InteractionState newState)
    {
        CurrentState = newState;
    }

    public void HideMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.None;
#endif

    }

    public void ShowMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}

public enum InteractionState { None, ActionScreen, EvidenceBoard, Dialogue, CaseFile}