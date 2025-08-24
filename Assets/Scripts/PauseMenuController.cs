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
    public Button playButton;

    [Header("Scene Management")]
    public string mainMenuScene = "MainMenuScene";
    public string gamePlayScene = "GameplayScene";

    [Header("References")]
    public CaseBoardController caseboardController;

    private bool isPaused = true;

    void Start()
    {
        
        pauseMenuPanel.SetActive(true);
        caseBoardPanel.SetActive(false);

        
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

    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (caseBoardPanel.activeSelf)
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

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        caseBoardPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        caseBoardPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenCaseBoard()
    {
        pauseMenuPanel.SetActive(false);
        caseBoardPanel.SetActive(true);

        
        if (caseboardController != null)
        {
            
        }
    }

    public void ReturnToPauseMenu()
    {
        caseBoardPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);

        
        if (caseboardController != null)
        {
            caseboardController.CloseDetailPanel();
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void QuitToMainMenu()
    {
        
        Time.timeScale = 1f;

        
        SceneManager.LoadScene(mainMenuScene);
    }
}