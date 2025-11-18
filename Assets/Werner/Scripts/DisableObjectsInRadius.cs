using UnityEngine;

public class DisableMap2RiftsInRadius : MonoBehaviour
{
    [SerializeField] private float radius = 100f;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] map2Rifts;

    public bool cameFromRiftZone = false;

    private bool wasInside = false;

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);
        bool isInside = distance <= radius;

        if (!isInside && wasInside)
        {
            wasInside = false;
        }

        if (isInside && !wasInside)
        {
            wasInside = true;

            if (cameFromRiftZone)
            {
                DisableRifts();
                cameFromRiftZone = false;
            }
        }
    }

    private void DisableRifts()
    {
        foreach (GameObject rift in map2Rifts)
        {
            if (rift != null && rift.activeSelf)
            {
                rift.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
