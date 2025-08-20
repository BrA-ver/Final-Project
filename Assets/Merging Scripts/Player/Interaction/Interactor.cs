using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

public class Interactor : MonoBehaviour
{
    [SerializeField] Vector3 checkOffset;
    [SerializeField] float checkRadius;

    List<Interactable> interactables = new List<Interactable>();

    Interactable interactable;

    private void Update()
    {
        if (interactables.Count > 0f)
        {
            interactable = interactables[0];
        }
        else
            interactable = null;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (!interactable) return;

        interactable.Interact();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            this.interactables.Add(interactable);
            interactable.Highlight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            this.interactables.Remove(interactable);
            interactable.StopHighlight();
        }
    }
}
