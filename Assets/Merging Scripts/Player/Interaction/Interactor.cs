using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;

public class Interactor : MonoBehaviour
{
    [SerializeField] float checkDistance = 3f;
    [SerializeField] LayerMask interactbleLayer;
    Interactable interactable;

    private void Start()
    {
        GameEvents.onInteractStop += StopInteracting;
    }

    private void OnDisable()
    {
        GameEvents.onInteractStop -= StopInteracting;
    }

    private void Update()
    {
        // Do a raycast to find interactables
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, checkDistance, interactbleLayer))
        {
            Debug.Log(hit.collider.name);
            Interactable newInteractable = hit.collider.GetComponent<Interactable>();
            if (newInteractable == null) 
            {
                if (interactable)
                {
                    interactable.StopHighlight();
                    HUD.instance.HideInteractIcon();
                }
                return; 
            }

            if (interactable == null)
            {
                interactable = newInteractable;
            }
            else if (interactable != newInteractable)
            {
                interactable.StopHighlight();
                HUD.instance.HideInteractIcon();
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

    private void StopInteracting()
    {
        GameManager.instance.IsInteracting = false;
        Debug.Log("stopped interacting");
    }

    

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (!interactable || GameManager.instance.IsInteracting) return;
        interactable.Interact();
    }

    
}
