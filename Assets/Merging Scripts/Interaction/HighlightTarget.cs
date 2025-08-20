using UnityEngine;

public class HighlightTarget : MonoBehaviour
{
    [SerializeField] MeshRenderer mesh;
    [SerializeField] Material outlineMaterial;
    [SerializeField] bool highlight;
    Material[] originalMaterials;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        originalMaterials = mesh.materials;
    }

    //private void Update()
    //{
    //    if (highlight)
    //        HighlightObject();
    //    else
    //        ClearHighlight();
    //}

    public void HighlightObject()
    {
        Material[] currentMaterials = originalMaterials;
        Material[] newMaterials = new Material[currentMaterials.Length + 1];

        for (int i = 0; i < currentMaterials.Length; i++)
        {
            newMaterials[i] = currentMaterials[i];
        }
        newMaterials[newMaterials.Length - 1] = outlineMaterial;
        mesh.materials = newMaterials;
    }

    public void ClearHighlight()
    {
        mesh.materials = originalMaterials;
    }

    public void Interact(GameObject interactor)
    {
        //throw new System.NotImplementedException();
    }
}
