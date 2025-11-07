using UnityEngine;

public class Interactable: MonoBehaviour
{
    [SerializeField] HighlightTarget[] targets;

    public void Highlight()
    {
        if (targets.Length <= 0) return;
        foreach (HighlightTarget target in targets)
            target.HighlightObject();
    }

    public void StopHighlight()
    {
        if (targets.Length <= 0 || targets == null) return;
        foreach (HighlightTarget target in targets)
            target.ClearHighlight();
    }

    public virtual void Interact()
    {
        Debug.Log("Interacted With " + name);
    }
}
