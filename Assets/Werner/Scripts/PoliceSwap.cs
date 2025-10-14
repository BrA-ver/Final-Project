using UnityEngine;
using System.Collections;

public class PoliceSwap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject walkingPoliceOfficer;
    [SerializeField] private GameObject idlePoliceOfficer;

    [Header("Delay before swapping (seconds)")]
    [SerializeField] private float delay = 1f;

    private bool hasSwapped = false;

    private void OnEnable()
    {
        // Trigger the swap when this script activates
        StartCoroutine(SwapAfterDelay());
    }

    private IEnumerator SwapAfterDelay()
    {
        if (hasSwapped) yield break; // Prevent double triggers

        hasSwapped = true;
        yield return new WaitForSeconds(delay);

        if (walkingPoliceOfficer != null)
            walkingPoliceOfficer.SetActive(false);

        if (idlePoliceOfficer != null)
            idlePoliceOfficer.SetActive(true);
    }
}
