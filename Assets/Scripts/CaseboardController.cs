using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static CaseBoardController;

public class CaseBoardController : MonoBehaviour
{
    [System.Serializable]
    public class CaseItem
    {
        public Button itemButton;
        public Image itemImage;
        public Text itemTitleText;
        public GameObject detailPanel;
        public Image detailImage;
        public Text detailDescriptionText;
        public string itemTitle;
        [TextArea] public string itemDescription;
        public Sprite itemSprite;

        [HideInInspector] public bool isDetailShown = false;
    }

    public CaseItem[] caseItems;
    private CaseItem currentlyShownItem = null;

    void Start()
    {
       
        foreach (var item in caseItems)
        {
            
            item.itemButton.onClick.AddListener(() => ToggleDetails(item));

            
            if (item.itemImage != null && item.itemSprite != null)
            {
                item.itemImage.sprite = item.itemSprite;
                item.itemImage.preserveAspect = true;
            }

            if (item.itemTitleText != null)
            {
                item.itemTitleText.text = item.itemTitle;
            }

            if (item.detailPanel != null)
            {
                item.detailPanel.SetActive(false);

               
                if (item.detailImage != null)
                {
                    item.detailImage.sprite = item.itemSprite;
                    item.detailImage.preserveAspect = true;
                }

                if (item.detailDescriptionText != null)
                {
                    item.detailDescriptionText.text = item.itemDescription;
                }
            }
        }
    }

    public void ToggleDetails(CaseItem selectedItem)
    {
        
        if (currentlyShownItem == selectedItem)
        {
            selectedItem.detailPanel.SetActive(!selectedItem.isDetailShown);
            selectedItem.isDetailShown = !selectedItem.isDetailShown;
            return;
        }

        
        if (currentlyShownItem != null)
        {
            currentlyShownItem.detailPanel.SetActive(false);
            currentlyShownItem.isDetailShown = false;
        }

        
        selectedItem.detailPanel.SetActive(true);
        selectedItem.isDetailShown = true;
        currentlyShownItem = selectedItem;
    }
}