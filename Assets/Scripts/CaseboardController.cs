using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;



public class CaseBoardController : MonoBehaviour
{
    [System.Serializable]
    public class CaseItemData
    {
        public Button itemButton;
        public Image itemImage;
        public Text itemTitleText;
        public Sprite itemSprite;
        public string itemTitle;
        [TextArea] public string itemDescription;
        [HideInInspector] public bool isDetailShown = false;
    }

    [Header("Case Items")]
    public CaseItemData[] caseItems;

    [Header("UI References")]
    public Transform caseItemsContainer;
    public GameObject caseItemPrefab;

    [Header("Shared Detail Panel")]
    public GameObject detailPanel;
    public Image detailImage;
    public Text detailTitleText;
    public Text detailDescriptionText;

    private CaseItemData currentlyShownItem = null;

    void Start()
    {
        
        ClearAllCaseItems();
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
        }
    }

    public void AddCaseItem(Sprite photoSprite, string itemTitle, string itemDescription)
    {
        
        CaseItemData newCaseItem = new CaseItemData();
        newCaseItem.itemSprite = photoSprite;
        newCaseItem.itemTitle = itemTitle;
        newCaseItem.itemDescription = itemDescription;

        
        GameObject newCaseItemObj = Instantiate(caseItemPrefab, caseItemsContainer);

        
        newCaseItem.itemButton = newCaseItemObj.GetComponent<Button>();
        newCaseItem.itemImage = newCaseItemObj.transform.Find("Photo").GetComponent<Image>();
        newCaseItem.itemTitleText = newCaseItemObj.transform.Find("Title").GetComponent<Text>();

        
        newCaseItem.itemImage.sprite = photoSprite;
        newCaseItem.itemImage.preserveAspect = true;
        newCaseItem.itemTitleText.text = itemTitle;

       
        newCaseItem.itemButton.onClick.AddListener(() => ToggleDetails(newCaseItem));

        
        CaseItemData[] newArray = new CaseItemData[caseItems.Length + 1];
        caseItems.CopyTo(newArray, 0);
        newArray[caseItems.Length] = newCaseItem;
        caseItems = newArray;
    }

    public void ToggleDetails(CaseItemData selectedItem)
    {
        if (currentlyShownItem == selectedItem && detailPanel.activeSelf)
        {
            
            detailPanel.SetActive(false);
            currentlyShownItem.isDetailShown = false;
            currentlyShownItem = null;
            return;
        }

        
        if (detailImage != null)
        {
            detailImage.sprite = selectedItem.itemSprite;
            detailImage.preserveAspect = true;
        }

        if (detailTitleText != null)
        {
            detailTitleText.text = selectedItem.itemTitle;
        }

        if (detailDescriptionText != null)
        {
            detailDescriptionText.text = selectedItem.itemDescription;
        }

       
        detailPanel.SetActive(true);

        
        if (currentlyShownItem != null)
        {
            currentlyShownItem.isDetailShown = false;
        }

        selectedItem.isDetailShown = true;
        currentlyShownItem = selectedItem;
    }

    public void ClearAllCaseItems()
    {
        
        foreach (Transform child in caseItemsContainer)
        {
            Destroy(child.gameObject);
        }

       
        caseItems = new CaseItemData[0];

        
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
        }

        currentlyShownItem = null;
    }

    
    public void CloseDetailPanel()
    {
        if (detailPanel != null)
        {
            detailPanel.SetActive(false);
        }

        if (currentlyShownItem != null)
        {
            currentlyShownItem.isDetailShown = false;
        }

        currentlyShownItem = null;
    }
}