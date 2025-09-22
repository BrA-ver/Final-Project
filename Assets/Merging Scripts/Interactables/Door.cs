using UnityEngine;
using System.Collections;

public class Door : Interactable
{
    [SerializeField] float closedAngle = 0f;
    [SerializeField] float openAngle = 90f;
    [SerializeField] float turnTime = 2f;
    bool isOpen;
    bool turning;


    private void Start()
    {
        closedAngle = transform.localRotation.eulerAngles.y;
    }

    public override void Interact()
    {
        base.Interact();
        ToggleDoor();
    }
    void ToggleDoor()
    {
        if (turning) return;

        turning = true;
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    void Open()
    {
        StartCoroutine(TurnDoorRoutine(openAngle));
    }

    void Close()
    {
        StartCoroutine(TurnDoorRoutine(closedAngle));
    }

    IEnumerator TurnDoorRoutine(float targetAngle)
    {
        float timePassed = 0f;
        Quaternion currentAngle = transform.localRotation;
        while (timePassed < turnTime)
        {
            timePassed += Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, transform.localRotation.eulerAngles.z);
            transform.localRotation = Quaternion.Slerp(currentAngle, targetRotation, timePassed / turnTime);
            yield return null;
        }

        isOpen = !isOpen;
        turning = false;
    }
}
