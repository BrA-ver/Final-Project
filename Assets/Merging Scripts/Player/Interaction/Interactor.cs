using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

public class Interactor : MonoBehaviour
{
    [SerializeField] float checkDistance = 3f;
    [SerializeField] LayerMask interactbleLayer;
    Interactable interactable;

    

    private void Update()
    {
        // Do a raycast to find interactables
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, checkDistance))
        {
            Interactable newInteractable = hit.collider.GetComponent<Interactable>();
            if (newInteractable == null) 
            {
                if (interactable)
                    interactable.StopHighlight();
                return; 
            }

            if (interactable == null)
            {
                interactable = newInteractable;
            }
            else if (interactable != newInteractable)
            {
                interactable.StopHighlight();
                interactable = newInteractable;
            }
            
            interactable.Highlight();
            HUD.instance.ShowInteractIcon();
            Debug.DrawLine(ray.origin, hit.point, Color.blue);
        }
        else
        {
            if (interactable)
                interactable.StopHighlight();
            HUD.instance.HideInteractIcon();
            interactable = null;
        }
    }

    ////[SerializeField] Vector3 checkOffset;
    ////[SerializeField] float checkRadius;

    //List<Interactable> interactables = new List<Interactable>();

    ////Interactable interactable;

    ////private void Update()
    ////{
    ////    if (interactables.Count > 0f)
    ////    {
    ////        interactable = interactables[0];
    ////    }
    ////    else
    ////        interactable = null;
    ////}

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (!interactable) return;

        interactable.Interact();
    }

    ////private void OnTriggerEnter(Collider other)
    ////{
    ////    if (other.TryGetComponent(out Interactable interactable))
    ////    {
    ////        this.interactables.Add(interactable);
    ////        interactable.Highlight();
    ////    }
    ////}

    ////private void OnTriggerExit(Collider other)
    ////{
    ////    if (other.TryGetComponent(out Interactable interactable))
    ////    {
    ////        this.interactables.Remove(interactable);
    ////        interactable.StopHighlight();
    ////    }
    ////}
}
