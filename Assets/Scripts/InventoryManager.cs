using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public InventoryData[] items;
    public InventoryData[] currentItems;

    [Header("Item Details")]
    public GameObject itemDetailsSection;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI Description;

    [Header("Inventory Slots")]
    public Button[] slots;

    [Header("Close Button")]
    public Button close;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        close.onClick.AddListener(() => gameObject.SetActive(false));
        LoadInventory();
    }

    void onDisable()
    {
        foreach (Button slot in slots)
        {
            slot.onClick.RemoveAllListeners();
        }
        close.onClick.RemoveListener(() => gameObject.SetActive(false));
    }

    public void LoadInventory()
    {
        currentItems = System.Array.FindAll(items, item => item.inInventory);

        for (int i =0; i < slots.Length; i++)
        {
            Image slotImage = slots[i].GetComponent<Image>();
            slots[i].onClick.RemoveAllListeners();

            if (i < currentItems.Length)
            {
                InventoryData item = currentItems[i];
                slotImage.sprite = item.itemImage;

                slots[i].onClick.AddListener(() => LoadItemDetails(item));
            }
            else
            {
                slotImage.sprite = null; 
            }
        }
    }

    public void LoadItemDetails(InventoryData item)
    {
        itemName.text = item.itemName;
        Description.text = item.itemDescription;
        itemImage.sprite = item.itemImage;
    }
}
