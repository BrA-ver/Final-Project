using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RoadSetup
{
    public GameObject carPrefab;
    public Transform startPoint;
    public List<Transform> breakPoints;
    public Transform endPoint;
}

public class CarManager : MonoBehaviour
{
    [Header("Roads Setup (1 per car)")]
    public List<RoadSetup> roads = new List<RoadSetup>();

    private int carsRemaining;  // how many cars are still alive
    private List<GameObject> activeCars = new List<GameObject>();

    private void Start()
    {
        SpawnAllCars();
    }

    private void SpawnAllCars()
    {
        activeCars.Clear();
        carsRemaining = roads.Count;

        foreach (var road in roads)
        {
            GameObject car = Instantiate(road.carPrefab, road.startPoint.position, road.startPoint.rotation);
            activeCars.Add(car);

            NavMeshAgent agent = car.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                CarController controller = car.AddComponent<CarController>();
                controller.Setup(this, agent, road.breakPoints, road.endPoint.position, car);
            }
        }
    }

    public void OnCarDestroyed(GameObject car)
    {
        if (activeCars.Contains(car))
            activeCars.Remove(car);

        carsRemaining--;

        if (carsRemaining <= 0)
        {
            // All cars finished -> respawn all
            SpawnAllCars();
        }
    }
}

public class CarController : MonoBehaviour
{
    private CarManager manager;
    private NavMeshAgent agent;
    private List<Vector3> breakPoints = new List<Vector3>();
    private Vector3 endPoint;
    private GameObject car;

    private int currentBreakIndex = 0;
    private bool isWaiting = false;

    public void Setup(CarManager manager, NavMeshAgent agent, List<Transform> breakPointTransforms, Vector3 endPoint, GameObject car)
    {
        this.manager = manager;
        this.agent = agent;
        this.endPoint = endPoint;
        this.car = car;

        foreach (Transform t in breakPointTransforms)
        {
            if (t != null)
                breakPoints.Add(t.position);
        }

        // start moving
        if (breakPoints.Count > 0)
            agent.SetDestination(breakPoints[0]);
        else
            agent.SetDestination(endPoint);
    }

    private void Update()
    {
        if (agent == null || agent.pathPending || isWaiting) return;

        // Handle break points
        if (currentBreakIndex < breakPoints.Count && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAtBreak(2f)); // <-- fixed 2 second delay (can be made adjustable per break later)
        }
        // Handle end point
        else if (currentBreakIndex >= breakPoints.Count && agent.remainingDistance <= agent.stoppingDistance)
        {
            manager.OnCarDestroyed(car);
            Destroy(car);
        }
    }

    private IEnumerator WaitAtBreak(float delay)
    {
        isWaiting = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(delay);

        agent.isStopped = false;
        currentBreakIndex++;

        if (currentBreakIndex < breakPoints.Count)
            agent.SetDestination(breakPoints[currentBreakIndex]);
        else
            agent.SetDestination(endPoint);

        isWaiting = false;
    }
}
