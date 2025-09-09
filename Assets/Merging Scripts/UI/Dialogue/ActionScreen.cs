using UnityEngine;
using UnityEngine.UI;

public class ActionScreen : MonoBehaviour
{
    public static ActionScreen instance;

    [SerializeField] GameObject holder;
    [SerializeField] Button firstButton;

    private void Awake()
    {
        instance = this;
    }

    public void ShowActions()
    {
        holder.SetActive(true);
        firstButton.Select();
    }
}
