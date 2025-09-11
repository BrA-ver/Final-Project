using UnityEngine;
using UnityEngine.AI;

public class PoliceNavMesh : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] movePoints;  // List of waypoints
    [SerializeField] private float pointReachedThreshold = 0.5f; // How close before switching

    private NavMeshAgent navMeshAgent;
    private int currentPointIndex = 0;
    private bool finishedPatrol = false;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (movePoints.Length > 0)
        {
            navMeshAgent.SetDestination(movePoints[currentPointIndex].position);
        }
    }

    private void Update()
    {
        if (finishedPatrol || movePoints.Length == 0) return;

        // Check if police has reached the current point
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= pointReachedThreshold)
        {
            currentPointIndex++;

            if (currentPointIndex < movePoints.Length)
            {
                navMeshAgent.SetDestination(movePoints[currentPointIndex].position);
            }
            else
            {
                finishedPatrol = true; // Stop moving at the last point
                navMeshAgent.ResetPath();
            }
        }
    }
}
