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
                movementScript.enabled = false;
                yield return null;
                movementScript.enabled = true;
            }
        }
    }
}
