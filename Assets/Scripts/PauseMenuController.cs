using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject caseBoardPanel;

    [Header("Navigation Buttons")]
    public Button resumeButton;
    public Button caseBoardButton;
    public Button backButton;
    public Button quitToMenuButton;

    [Header("Scene Management")]
    public string mainMenuScene = "MainMenuScene";

    [Header("References")]
    public CaseBoardController caseboardController;

    [Header("Canvas Settings")]
    public Canvas canvas;
    public bool setToFrontLayer = true;

    private bool isPaused = false;

    void Awake()
    {
        // Check for multiple instances and destroy duplicates
        PauseMenuController[] controllers = FindObjectsByType<PauseMenuController>(FindObjectsSortMode.None);
        if (controllers.Length > 1)
        {
            Debug.LogWarning("Multiple PauseMenuController instances found! Destroying duplicate: " + gameObject.name);
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Debug.Log("PauseMenuController Start() called in scene: " + SceneManager.GetActiveScene().name);

        
        Time.timeScale = 1f;

        // Auto-find canvas if not assigned
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindAnyObjectByType<Canvas>();
            }
            if (canvas != null)
            {
                Debug.Log("Auto-found canvas: " + canvas.name);
            }
        }

        
        if (!ValidateReferences())
        {
            Debug.LogError("PauseMenuController references are missing! Pause menu will not work properly.");
            return;
        }

        
        ForceUISetup();

        // Setup button listeners
        resumeButton.onClick.AddListener(ResumeGame);
        caseBoardButton.onClick.AddListener(OpenCaseBoard);
        backButton.onClick.AddListener(ReturnToPauseMenu);

        
        resumeButton.interactable = true;
        caseBoardButton.interactable = true;
        backButton.interactable = true;

        if (quitToMenuButton != null)
        {
            quitToMenuButton.onClick.AddListener(QuitToMainMenu);
        }
        else
        {
            Debug.LogWarning("QuitToMenuButton is not assigned in the Inspector.");
        }

        // Initial cursor state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;

        Debug.Log("PauseMenuController initialized successfully in scene: " + SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        // Test input detection
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P key pressed - Input system is working!");
            Debug.Log("Current pause state: " + isPaused);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed - IsPaused: " + isPaused + ", CaseBoard active: " + (caseBoardPanel != null && caseBoardPanel.activeSelf));

            if (caseBoardPanel != null && caseBoardPanel.activeSelf)
            {
                ReturnToPauseMenu();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void ForceUISetup()
    {
        // Ensure panels are properly set up
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);

            
            pauseMenuPanel.transform.SetAsLastSibling();

            
            RectTransform rect = pauseMenuPanel.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }
        }

        if (caseBoardPanel != null)
        {
            caseBoardPanel.SetActive(false);
            caseBoardPanel.transform.SetAsLastSibling();
        }

        // Set canvas to front layer
        if (canvas != null && setToFrontLayer)
        {
            canvas.sortingOrder = 999;
            canvas.overrideSorting = true;
        }
    }

    private bool ValidateReferences()
    {
        bool allValid = true;

        if (pauseMenuPanel == null)
        {
            Debug.LogError("PauseMenuPanel is not assigned in the Inspector!");
            allValid = false;
        }
        else
        {
            Debug.Log("PauseMenuPanel reference is valid: " + pauseMenuPanel.name);
        }

        if (caseBoardPanel == null)
        {
            Debug.LogError("CaseBoardPanel is not assigned in the Inspector!");
            allValid = false;
        }
        else
        {
            Debug.Log("CaseBoardPanel reference is valid: " + caseBoardPanel.name);
        }

        if (resumeButton == null)
        {
            Debug.LogError("ResumeButton is not assigned in the Inspector!");
            allValid = false;
        }

        if (caseBoardButton == null)
        {
            Debug.LogError("CaseBoardButton is not assigned in the Inspector!");
            allValid = false;
        }

        if (backButton == null)
        {
            Debug.LogError("BackButton is not assigned in the Inspector!");
            allValid = false;
        }

        return allValid;
    }

    public void PauseGame()
    {
        Debug.Log("Attempting to pause game...");

        if (pauseMenuPanel == null)
        {
            Debug.LogError("Cannot pause: pauseMenuPanel is null!");
            return;
        }

        Time.timeScale = 0f;
        isPaused = true;

        // Force the pause menu to be visible and on top
        pauseMenuPanel.SetActive(true);
        pauseMenuPanel.transform.SetAsLastSibling();

        
        CanvasGroup canvasGroup = pauseMenuPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (caseBoardPanel != null)
        {
            caseBoardPanel.SetActive(false);
        }

        
        if (canvas != null)
        {
            canvas.enabled = true;
            if (setToFrontLayer)
            {
                canvas.sortingOrder = 999;
            }
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Game paused successfully - Pause menu should be visible");
        Debug.Log("PauseMenuPanel active: " + pauseMenuPanel.activeInHierarchy);

        // Force a UI rebuild
        LayoutRebuilder.ForceRebuildLayoutImmediate(pauseMenuPanel.GetComponent<RectTransform>());
    }

    public void ResumeGame()
    {
        Debug.Log("Attempting to resume game...");

        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (caseBoardPanel != null)
        {
            caseBoardPanel.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Game resumed successfully");
    }

    public void OpenCaseBoard()
    {
        Debug.Log("Opening case board...");

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        if (caseBoardPanel != null)
        {
            caseBoardPanel.SetActive(true);
            caseBoardPanel.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogError("Cannot open case board: caseBoardPanel is null!");
            return;
        }

        
        if (caseboardController != null)
        {
            Debug.Log("Case board controller reference is available");
        }
        else
        {
            Debug.LogWarning("CaseBoardController reference is not assigned.");
        }
    }

    public void ReturnToPauseMenu()
    {
        Debug.Log("Returning to pause menu...");

        if (caseBoardPanel != null)
        {
            caseBoardPanel.SetActive(false);
        }

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            pauseMenuPanel.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogError("Cannot return to pause menu: pauseMenuPanel is null!");
            return;
        }

        
        if (caseboardController != null)
        {
            caseboardController.CloseDetailPanel();
        }

        Debug.Log("Returned to pause menu successfully");
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void QuitToMainMenu()
    {
        Debug.Log("Quitting to main menu...");

        // Reset time scale
        Time.timeScale = 1f;

        // Load main menu scene
        if (!string.IsNullOrEmpty(mainMenuScene))
        {
            SceneManager.LoadScene(mainMenuScene);
        }
        else
        {
            Debug.LogError("Main menu scene name is not set!");
        }
    }

    // Public method to check if controller is properly initialized
    public bool IsInitialized()
    {
        return pauseMenuPanel != null &&
               caseBoardPanel != null &&
               resumeButton != null &&
               caseBoardButton != null &&
               backButton != null;
    }

    // Debug method to manually show the pause menu
    [ContextMenu("Force Show Pause Menu")]
    public void ForceShowPauseMenu()
    {
        Debug.Log("Forcing pause menu to show...");
        PauseGame();
    }
}