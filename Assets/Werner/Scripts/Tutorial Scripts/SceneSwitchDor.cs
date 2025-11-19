using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchDoor : Interactable
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private HighlightTarget highlightTarget;

    private bool isPlayerNearby = false;

    public override void Interact()
    {
        base.Interact();

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            // ⭐ Next scene starts with a cutscene
            CutsceneManager.IsCutsceneActive = true;

            // ⭐ Disable input so it cannot lock the cursor mid-load
            if (InputHandler.instance != null)
                InputHandler.instance.enabled = false;

            // ⭐ Ensure cursor visible BEFORE transition
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene(sceneToLoad);
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
