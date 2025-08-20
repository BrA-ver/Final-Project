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
        public GameObject detailPanel;
        public Image detailImage;
        public Text detailDescriptionText;
        [HideInInspector] public bool isDetailShown = false;

        [Header("UI References")]
        public Transform caseItemsContainer;  
        public GameObject caseItemPrefab;     
    }

    [Header("UI References")]
    public Transform caseItemsContainer;
    public GameObject caseItemPrefab;

    private List<CaseItemData> caseItems = new List<CaseItemData>();
    private CaseItemData currentlyShownItem = null;

    void Start()
    {
        
        ClearAllCaseItems();
    }

    public void AddCaseItem(Sprite photoSprite, string itemTitle, string itemDescription)
    {
        
        GameObject newCaseItemObj = Instantiate(caseItemPrefab, caseItemsContainer);
        CaseItemData newCaseItem = new CaseItemData();

        
        newCaseItem.itemButton = newCaseItemObj.GetComponent<Button>();
        newCaseItem.itemImage = newCaseItemObj.transform.Find("Photo").GetComponent<Image>();
        newCaseItem.itemTitleText = newCaseItemObj.transform.Find("Title").GetComponent<Text>();
        newCaseItem.detailPanel = newCaseItemObj.transform.Find("DetailPanel").gameObject;
        newCaseItem.detailImage = newCaseItem.detailPanel.transform.Find("DetailImage").GetComponent<Image>();
        newCaseItem.detailDescriptionText = newCaseItem.detailPanel.transform.Find("Description").GetComponent<Text>();

       
        newCaseItem.itemImage.sprite = photoSprite;
        newCaseItem.itemImage.preserveAspect = true;
        newCaseItem.itemTitleText.text = itemTitle;
        newCaseItem.detailImage.sprite = photoSprite;
        newCaseItem.detailImage.preserveAspect = true;
        newCaseItem.detailDescriptionText.text = itemDescription;

        
        newCaseItem.itemButton.onClick.AddListener(() => ToggleDetails(newCaseItem));

        
        newCaseItem.detailPanel.SetActive(false);

        
        caseItems.Add(newCaseItem);
    }

    public void ToggleDetails(CaseItemData selectedItem)
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

    public void ClearAllCaseItems()
    {
        
        foreach (Transform child in caseItemsContainer)
        {
            Destroy(child.gameObject);
        }

        caseItems.Clear();
        currentlyShownItem = null;
    }
}