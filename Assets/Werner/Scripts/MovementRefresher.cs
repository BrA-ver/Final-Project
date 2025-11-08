using UnityEngine;
using System.Collections;

public class MovementRefresher : MonoBehaviour
{
    [SerializeField] private WernerMovement movementScript;
    [SerializeField] private float refreshInterval = 1f;

    private void Start()
    {
        if (movementScript == null)
            movementScript = GetComponent<WernerMovement>();

        StartCoroutine(RefreshMovementRoutine());
    }

    private IEnumerator RefreshMovementRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(refreshInterval);

            if (movementScript != null)
            {
                // Disable and re-enable the script to force Unity to reinitialize it
                movementScript.enabled = false;
                yield return null; // wait 1 frame
                movementScript.enabled = true;
            }
        }
    }
}
