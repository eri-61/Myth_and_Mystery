using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryManager : MonoBehaviour
{
    #region Variables
    public static InventoryManager Instance { private set; get; }

    [Header("Item detals")]
    public GameObject itemDetailsSection;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI Description;

    [Header("Item Slots")]
    //public Button[] slots;
    [SerializeField] private Button slots1;
    [SerializeField] private Button slots2;
    [SerializeField] private Button slots3;
    [SerializeField] private Button slots4;
    [SerializeField] private Button slots5;
    [SerializeField] private Button slots6;
    [SerializeField] private Button slots7;
    [SerializeField] private Button slots8;
    [SerializeField] private Button slots9;

    [Header("Nav Bar")]
    public Button close;
    public Button useItem;

    private List<ItemData> currentItems = new List<ItemData>();
    [HideInInspector] public ItemData selectedItem;
    #endregion

    public event Action<ItemData> OnUseItemReq;
    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        close.onClick.AddListener(CloseInv);
        useItem.onClick.AddListener(OnUseItem);
    }

    private void OnDisable()
    {
        close.onClick.RemoveAllListeners();
        useItem.onClick.RemoveAllListeners();
    }

    public void LoadInventoryUI()
    {
        int idx = 1;

        foreach (var itm in currentItems)
        {
            if (idx == 1)
            {
                Image slotImage1 = slots1.GetComponent<Image>();
                slots1.onClick.RemoveAllListeners();
                ItemData item = currentItems[idx - 1];
                slotImage1.sprite = item.itemSprite;
                slots1.onClick.AddListener(() => ShowItemDetails(item));
            }
            if (idx == 2)
            {
                Image slotImage2 = slots2.GetComponent<Image>();
                slots2.onClick.RemoveAllListeners();
                ItemData item = currentItems[idx - 1];
                slotImage2.sprite = item.itemSprite;
                slots2.onClick.AddListener(() => ShowItemDetails(item));
            }
            if (idx == 3)
            {
                Image slotImage3 = slots3.GetComponent<Image>();
                slots3.onClick.RemoveAllListeners();
                ItemData item = currentItems[idx - 1];
                slotImage3.sprite = item.itemSprite;
                slots3.onClick.AddListener(() => ShowItemDetails(item));
            }
            if (idx == 4)
            {
                Image slotImage4 = slots4.GetComponent<Image>();
                slots4.onClick.RemoveAllListeners();
                ItemData item = currentItems[idx - 1];
                slotImage4.sprite = item.itemSprite;
                slots4.onClick.AddListener(() => ShowItemDetails(item));
            }
            if (idx == 5)
            {
                Image slotImage5 = slots5.GetComponent<Image>();
                slots5.onClick.RemoveAllListeners();
                ItemData item = currentItems[idx - 1];
                slotImage5.sprite = item.itemSprite;
                slots5.onClick.AddListener(() => ShowItemDetails(item));
            }
            if (idx == 6)
            {
                Image slotImage6 = slots6.GetComponent<Image>();
                slots6.onClick.RemoveAllListeners();
                ItemData item = currentItems[idx - 1];
                slotImage6.sprite = item.itemSprite;
                slots6.onClick.AddListener(() => ShowItemDetails(item));
            }
            if (idx == 7)
            {
                Image slotImage7 = slots7.GetComponent<Image>();
                slots7.onClick.RemoveAllListeners();
                ItemData item = currentItems[idx - 1];
                slotImage7.sprite = item.itemSprite;
                slots7.onClick.AddListener(() => ShowItemDetails(item));
            }
            if (idx == 8)
            {
                Image slotImage8 = slots8.GetComponent<Image>();
                slots8.onClick.RemoveAllListeners();
                ItemData item = currentItems[idx - 1];
                slotImage8.sprite = item.itemSprite;
                slots8.onClick.AddListener(() => ShowItemDetails(item));
            }
            if (idx == 9)
            {
                Image slotImage9 = slots9.GetComponent<Image>();
                slots9.onClick.RemoveAllListeners();
                ItemData item = currentItems[idx - 1];
                slotImage9.sprite = item.itemSprite;
                slots9.onClick.AddListener(() => ShowItemDetails(item));
            }
            idx++;
        }           


    }

    private void ShowItemDetails(ItemData item)
    {
        selectedItem = item;
        itemName.text = item.itemName;
        Description.text = item.itemDescription;
        itemImage.sprite = item.itemSprite;
        itemDetailsSection.SetActive(true);
    }
 
    public void AddItem(ItemData newItem)
    {
        if (newItem == null)
        {
            Debug.LogWarning("[InventoryManager] AddItem called with null.");
            return;
        }

        newItem.inInventory = true;

        if (!currentItems.Contains(newItem))
            currentItems.Add(newItem);

        LoadInventoryUI();

        Debug.Log($"[InventoryManager] Added item: {newItem.itemName}");
    }

    void CloseInv()
    {
        PersistentObjects.instance.CloseInventory();
    }

    private void OnUseItem()
    {
        if (selectedItem == null)
        {
            return;
        }
        OnUseItemReq?.Invoke(selectedItem);
        CloseInv();
    }
}
