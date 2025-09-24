using UnityEngine;
using UnityEngine.EventSystems;

public class GameButton : MonoBehaviour
{
    public void Deselect()
    {
        // Trigger OnDeselect before clearing
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected != null)
        {
            ExecuteEvents.Execute<IDeselectHandler>(
                selected,
                new BaseEventData(EventSystem.current),
                ExecuteEvents.deselectHandler
            );
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Selected()
    {
        Debug.Log("Selected " + name);
    }

    public void Deselected()
    {
        Debug.Log("Deselected " + name);
    }
}
