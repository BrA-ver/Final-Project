using UnityEngine;

public class EnableOnClueInteract : EvidenceObject
{
    [Header("Objects to Enable")]
    [SerializeField] private GameObject[] objectsToEnable;

    public override void Interact()
    {
        // 🔹 Call the base EvidenceObject logic first (collect evidence, hide if needed)
        base.Interact();

        // 🔹 Enable all assigned objects
        foreach (GameObject obj in objectsToEnable)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                Debug.Log($"Enabled object: {obj.name}");
            }
        }
    }
}
