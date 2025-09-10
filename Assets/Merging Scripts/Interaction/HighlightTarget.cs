using UnityEngine;

public class HighlightTarget : MonoBehaviour
{
    [SerializeField] MeshRenderer mesh;
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] Material outlineMaterial;
    [SerializeField] bool highlight;
    Material[] originalMaterials;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        if (!mesh)
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            originalMaterials = skinnedMeshRenderer.materials;
        }
        else
        {
            originalMaterials = mesh.materials;
        }
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

        if (mesh)
            mesh.materials = newMaterials;
        else
            skinnedMeshRenderer.materials = newMaterials;
    }

    public void ClearHighlight()
    {
        if (mesh)
            mesh.materials = originalMaterials;
        else
            skinnedMeshRenderer.materials = originalMaterials;
    }

    public void Interact(GameObject interactor)
    {
        //throw new System.NotImplementedException();
    }
}

[System.Serializable]
public class MeshHighlight
{
    public MeshRenderer mesh;
    Material[] originalMaterials;

    public void SetOriginal()
    {
        Material[] currentMaterials = originalMaterials;
    }
}
