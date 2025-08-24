using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string gameplaySceneName = "GameplayScene"; // Change to your friend's scene name

    [Header("UI Reference (Optional)")]
    public Button playButton;

    void Start()
    {
        // Automatically setup button listener if assigned
        if (playButton != null)
        {
            playButton.onClick.AddListener(LoadGameplayScene);
        }
    }

    // Call this method from your Play button
    public void LoadGameplayScene()
    {
        Debug.Log("Loading gameplay scene: " + gameplaySceneName);

        // Load the gameplay scene
        SceneManager.LoadScene(gameplaySceneName);
    }
}
