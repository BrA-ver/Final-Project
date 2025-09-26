using UnityEngine;

public class Interactable: MonoBehaviour
{
    [SerializeField] HighlightTarget[] targets;

    public void Highlight()
    {
        foreach (HighlightTarget target in targets)
            target.HighlightObject();
    }

    public void StopHighlight()
    {
        foreach(HighlightTarget target in targets)
            target.ClearHighlight();
    }

    public virtual void Interact()
    {
        //Debug.Log("Interacted With " + name);
    }
}
