using UnityEngine;

public class TutorialRadiusUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject tutorialUI;

    [Header("Settings")]
    [SerializeField] private float showRadius = 5f;

    private bool uiIsActive = false;

    void Update()
    {
        if (player == null || tutorialUI == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // Player IN radius
        if (distance <= showRadius)
        {
            if (!uiIsActive)
            {
                tutorialUI.SetActive(true);
                uiIsActive = true;
            }
        }
        // Player OUT of radius
        else
        {
            if (uiIsActive)
            {
                tutorialUI.SetActive(false);
                uiIsActive = false;
            }
        }
    }
}
