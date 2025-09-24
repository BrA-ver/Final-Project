using UnityEngine;

public class HighlightTarget : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    [SerializeField] Material outlineMaterial;
    [SerializeField] bool highlight;
    Material[] originalMaterials;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        originalMaterials = renderer.materials;
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

        renderer.materials = newMaterials;
    }

    public void ClearHighlight()
    {
        renderer.materials = originalMaterials;
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
