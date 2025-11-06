using UnityEngine;

public class DisableObjectsInRadius : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Target Object (Center of Radius Check)")]
    [SerializeField] private Transform targetObject;

    [Header("Radius Settings")]
    [SerializeField] private float disableRadius = 100f;

    [Header("Objects To Disable When In Radius")]
    [SerializeField] private GameObject[] objectsToDisable;

    void Update()
    {
        if (player == null || targetObject == null) return;

        float distance = Vector3.Distance(player.position, targetObject.position);
        bool isInRadius = distance <= disableRadius;

        foreach (GameObject obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(!isInRadius); // Disable if inside radius, enable if outside
        }
    }
}
