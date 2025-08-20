using UnityEngine;

public class Interactable: MonoBehaviour
{
    HighlightTarget target;

    private void Awake()
    {
        target = GetComponent<HighlightTarget>();
    }

    public void Highlight()
    {
        target.HighlightObject();
    }

    public void StopHighlight()
    {
        target.ClearHighlight();
    }

    public virtual void Interact()
    {
        Debug.Log("Interacted With " + name);
    }
}
