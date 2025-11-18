using UnityEngine; 
using UnityEngine.AI;
using System.Collections;

public class PoliceNavMesh : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] movePoints;
    [SerializeField] private float pointReachedThreshold = 0.5f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    [Header("Finish Rotation")]
    [SerializeField] private bool rotateOnFinish = true;
    [SerializeField] private float finishYawDegrees = 180f;
    [SerializeField] private float rotateDuration = 0.25f;

    [Header("Optional Components")]
    [SerializeField] private MonoBehaviour npcScriptToRemove;

    private NavMeshAgent navMeshAgent;
    private int currentPointIndex = 0;
    private bool finishedPatrol = true;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
    }

    private void Update()
    {
        if (finishedPatrol || movePoints.Length == 0) return;

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= pointReachedThreshold)
        {
            currentPointIndex++;

            if (currentPointIndex < movePoints.Length)
            {
                navMeshAgent.SetDestination(movePoints[currentPointIndex].position);
            }
            else
            {
                finishedPatrol = true;
                navMeshAgent.isStopped = true;
                navMeshAgent.ResetPath();

                if (animator != null)
                    animator.SetBool(IsWalking, false);

                if (rotateOnFinish)
                {
                    float targetY = transform.eulerAngles.y + finishYawDegrees;
                    if (rotateDuration <= 0f)
                    {
                        transform.rotation = Quaternion.Euler(0f, targetY, 0f);
                        TriggerPoliceSwap();
                    }
                    else
                    {
                        StartCoroutine(RotateAndSwap(targetY, rotateDuration));
                    }
                }
                else
                {
                    TriggerPoliceSwap();
                }
            }
        }
    }

    public void StartPatrol()
    {
        if (movePoints == null || movePoints.Length == 0)
        {
            return;
        }

        finishedPatrol = false;
        currentPointIndex = 0;

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(movePoints[currentPointIndex].position);

        if (npcScriptToRemove != null)
        {
            Destroy(npcScriptToRemove);
        }

        if (animator != null)
            animator.SetBool(IsWalking, true);
    }

    private IEnumerator RotateAndSwap(float targetY, float duration)
    {
        Quaternion start = transform.rotation;
        Quaternion target = Quaternion.Euler(0f, targetY, 0f);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.rotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        transform.rotation = target;

        TriggerPoliceSwap();
    }

    private void TriggerPoliceSwap()
    {
        PoliceSwap swapper = GetComponent<PoliceSwap>();
        if (swapper != null)
        {
            swapper.enabled = true;
        }
    }
}
