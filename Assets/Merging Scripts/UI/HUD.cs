using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    [SerializeField] GameObject interactIcon;

    private void Awake()
    {
        instance = this;
        HideInteractIcon();
    }

    public void ShowInteractIcon()
    {
        interactIcon.SetActive(true);
    }

    public void HideInteractIcon()
    {
        interactIcon.SetActive(false);
    }
}
