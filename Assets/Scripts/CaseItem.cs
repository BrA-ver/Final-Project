using UnityEngine;
using UnityEngine.UI;

public class CaseItem : MonoBehaviour
{
    [Header("References")]
    public Image photo;
    public Text title;
    public GameObject detailPanel;
    public Text descriptionText;

    public void Initialize(Sprite photoSprite, string itemTitle, string itemDescription)
    {
        photo.sprite = photoSprite;
        title.text = itemTitle;
        descriptionText.text = itemDescription;

        
        GetComponent<Button>().onClick.AddListener(ToggleDetails);
    }

    public void ToggleDetails()
    {
        detailPanel.SetActive(!detailPanel.activeSelf);
    }
}