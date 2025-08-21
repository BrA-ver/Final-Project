using UnityEngine;
using UnityEngine.UI;

public class CaseBoardCloseButton : MonoBehaviour
{
    private Button closeButton;
    private PauseMenuController pauseMenuController;

    void Start()
    {
        closeButton = GetComponent<Button>();
        pauseMenuController = FindObjectOfType<PauseMenuController>();

        if (closeButton != null && pauseMenuController != null)
        {
            closeButton.onClick.AddListener(pauseMenuController.ReturnToPauseMenu);
        }
    }
}
