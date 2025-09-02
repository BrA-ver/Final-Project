using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ProximityEdgeHighlight : MonoBehaviour
{
    [Header("Highlight Settings")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private float triggerDistance = 5f;
    [SerializeField] private Transform player;

    private MeshRenderer meshRenderer;
    private Material[] originalMaterials;
    private bool isHighlighted = false;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterials = meshRenderer.materials;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= triggerDistance && !isHighlighted)
        {
            HighlightObject();
        }
        else if (distance > triggerDistance && isHighlighted)
        {
            ClearHighlight();
        }
    }

    private void HighlightObject()
    {
        Material[] newMaterials = new Material[originalMaterials.Length + 1];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            newMaterials[i] = originalMaterials[i];
        }
        newMaterials[newMaterials.Length - 1] = outlineMaterial;
        meshRenderer.materials = newMaterials;
        isHighlighted = true;
    }

    private void ClearHighlight()
    {
        meshRenderer.materials = originalMaterials;
        isHighlighted = false;
    }
}
