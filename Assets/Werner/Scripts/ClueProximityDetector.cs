using UnityEngine;
using TMPro;

public class ClueProximityDetector : MonoBehaviour
{
    public float detectionRadius = 4f;
    public GameObject leftIndicator;
    public GameObject rightIndicator;
    public TextMeshProUGUI clueText;

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        bool clueNearby = false;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Clue"))
            {
                clueNearby = true;
                break;
            }
        }

        leftIndicator.SetActive(clueNearby);
        rightIndicator.SetActive(clueNearby);
        clueText.gameObject.SetActive(clueNearby);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Clue"))
            {
                Gizmos.DrawLine(transform.position, hit.transform.position);
            }
        }
    }
}
