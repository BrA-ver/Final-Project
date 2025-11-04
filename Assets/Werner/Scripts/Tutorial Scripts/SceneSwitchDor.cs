using UnityEngine;
using UnityEngine.SceneManagement; // ✅ Needed for scene loading

public class SceneSwitchDoor : Interactable
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad; // Name of the scene to load
    [SerializeField] private HighlightTarget highlightTarget;

    private bool isPlayerNearby = false;

    public override void Interact()
    {
        base.Interact();

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"✅ Loading scene: {sceneToLoad}");
            SceneManager.LoadScene(sceneToLoad); // 🔁 Changes scene
        }
        else
        {
            Debug.LogWarning("⚠ Scene name not assigned in Inspector!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;

            if (highlightTarget != null)
                highlightTarget.HighlightObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            if (highlightTarget != null)
                highlightTarget.ClearHighlight();
        }
    }
}
