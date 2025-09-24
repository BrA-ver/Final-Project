using UnityEngine;
using UnityEngine.AI;

public class CarTraffic : MonoBehaviour
{
    [Header("Driving Points (set these on the streets)")]
    [SerializeField] private Transform[] drivingPoints;

    [Header("Settings")]
    [SerializeField] private float reachThreshold = 1.5f;   // How close before switching points
    [SerializeField] private float minSpeed = 8f;
    [SerializeField] private float maxSpeed = 15f;

    private NavMeshAgent agent;
    private int currentIndex = 0;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (drivingPoints.Length == 0)
        {
            Debug.LogError("No driving points assigned for " + gameObject.name);
            enabled = false;
            return;
        }

        // Give each car some speed variety
        agent.speed = Random.Range(minSpeed, maxSpeed);

        // Start moving
        agent.SetDestination(drivingPoints[currentIndex].position);
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < reachThreshold)
        {
            GoToNextPoint();
        }
    }

    private void GoToNextPoint()
    {
        // Increase index and loop back to 0 when reaching the last point
        currentIndex = (currentIndex + 1) % drivingPoints.Length;
        agent.SetDestination(drivingPoints[currentIndex].position);
    }

    // Debug: visualize waypoints
    void OnDrawGizmosSelected()
    {
        if (drivingPoints == null || drivingPoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < drivingPoints.Length; i++)
        {
            if (drivingPoints[i] != null)
                Gizmos.DrawSphere(drivingPoints[i].position, 0.5f);

            if (i < drivingPoints.Length - 1 && drivingPoints[i] != null && drivingPoints[i + 1] != null)
                Gizmos.DrawLine(drivingPoints[i].position, drivingPoints[i + 1].position);
        }

        // Close the loop back to the first point
        if (drivingPoints[0] != null && drivingPoints[drivingPoints.Length - 1] != null)
            Gizmos.DrawLine(drivingPoints[drivingPoints.Length - 1].position, drivingPoints[0].position);
    }
}
