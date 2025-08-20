using UnityEngine;

public class DisablePlayerInputOnPause : MonoBehaviour
{
    public MonoBehaviour[] playerControllers;
    private PauseMenuController pauseMenu;

    void Start()
    {
        pauseMenu = Object.FindFirstObjectByType<PauseMenuController>();
    }

    void Update()
    {
        bool shouldDisable = pauseMenu != null && pauseMenu.IsPaused();

        foreach (var controller in playerControllers)
        {
            controller.enabled = !shouldDisable;
        }
    }

   
}