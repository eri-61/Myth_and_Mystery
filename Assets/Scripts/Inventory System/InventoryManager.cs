using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    #region Variables
    public static InventoryManager instance { private set; get; }
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    public Button journalButton;
    public Button settingsButton;

    int selectedSlot = -1;
    [HideInInspector] public List<Items> currentItems = new List<Items>();
    #endregion

    void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);


            //Check Game Save and re-apply found items
            var curGS = SaveManager.Instance.GetGameState();

            if (curGS.Inventory != null)
            {
                if (curGS.Inventory.Items != null)
                {
                    Debug.Log($"InvestigationScene > Reloading Inventory Items > Item Count: {curGS.Inventory.Items.Count}");
                    foreach (var itm in curGS.Inventory.Items)
                    {
                        Sprite incomingSprite = Resources.Load<Sprite>($"Assets/Game UI/Items/{itm.ItemName.ToLower()}");

                        Items loadedItem = ScriptableObject.CreateInstance<Items>();
                        loadedItem.name = itm.ItemName;
                        loadedItem.image = incomingSprite;
                        currentItems.Add(loadedItem);
                    }
                }
            }
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

    }

    public void CleanUpAndDestroy()
    {
        instance = null;
        Destroy(this.gameObject);
    }

    private void Start()
    {
        LoadInventoryUI();
    }

    private void OnEnable()
    {
        journalButton.onClick.AddListener(() =>
        {
            JournalManager.instance.OpenJournal();
        });
        settingsButton.onClick.AddListener(() =>
        {
            SettingsScript.instance.OpenSettings();
        });
    }

    private void OnDisable()
    {
        journalButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] != null)
            {
                inventorySlots[i].index = i;
            }
       }
    }

    public void ChangeSelectedSlot(int itemSlot)
    {
        // Deselect previous
        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Length)
        {
            inventorySlots[selectedSlot].Deselect();
        }

        selectedSlot = itemSlot;

        // Only select if valid
        if (selectedSlot >= 0 && selectedSlot < inventorySlots.Length)
        {
            inventorySlots[selectedSlot].Select();
        }
    }

    public void AddItem(Items item)
    {
        if (currentItems.Any(i => i.name == item.name))
        {
            Debug.LogWarning($"[InventoryManager] Item '{item.name}' already exists in inventory. Skipping addition.");
            return; 
        }

        for (int i = 0; i< inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if(itemInSlot == null)
            {
                SpawnNewItem(item, slot);

                var curGS = SaveManager.Instance.GetGameState();
                if(curGS != null)
                {
                    if(curGS.Inventory == null)
                    {
                        curGS.Inventory = new GameInventory();
                    }

                    if (curGS.Inventory.Items == null)
                    {
                        curGS.Inventory.Items = new List<GameItem>();
                    }

                    var existingItem = curGS.Inventory.Items.Where(c => c.ItemName == item.name);

                    if(existingItem == null && !string.IsNullOrWhiteSpace(item.name))
                    {
                        curGS.Inventory.Items.Add(new GameItem()
                        {
                            ItemName = item.name
                        });
                    }
                    LoadInventoryUI();    
                }
                return;
            }
        }
    }

    public void SpawnNewItem(Items item, InventorySlot slot)
    {
        GameObject newItemGO = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem itemInSlot = newItemGO.GetComponent<InventoryItem>();
        itemInSlot.InitialiseItem(item);
        currentItems.Add(item);
    }

    public string GetSelectedName()
    {
        if (selectedSlot < 0 || selectedSlot >= inventorySlots.Length)
        {
            return null;
        }

        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null && itemInSlot.item != null)
        {
            return itemInSlot.item.name;
        }

        return null;
    }

    public Items GetSelectedItem(bool use)
    {
        if (selectedSlot < 0 || selectedSlot >= inventorySlots.Length)
        {
            return null;
        }

        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

        if (itemInSlot != null)
        {
            Items item = itemInSlot.item;

            if (use == true)
            {
                itemInSlot.Count--;
                if (itemInSlot.Count <= 0)
                {
                    currentItems.Remove(item);
                    RemoveItemFromSave(item.name);
                    Destroy(itemInSlot.gameObject);

                    ChangeSelectedSlot(-1); 
                    LoadInventoryUI(); 
                    return item;
                }
            }
            return item;
        }
        return null;
    }

    public void RemoveItemFromSave(string itemName)
    {
        var curGS = SaveManager.Instance.GetGameState();

        if (curGS != null && curGS.Inventory != null && curGS.Inventory.Items != null)
        {
            var itemToRemove = curGS.Inventory.Items
                .FirstOrDefault(gi => gi.ItemName.Equals(itemName, System.StringComparison.OrdinalIgnoreCase));

            if (itemToRemove != null)
            {
                curGS.Inventory.Items.Remove(itemToRemove);
                Debug.Log($"[InventoryManager] Successfully removed item '{itemName}' from save data.");
            }
            else
            {
                Debug.LogWarning($"[InventoryManager] Item '{itemName}' not found in save data to remove.");
            }
        }
    }

    public void LoadInventoryUI()
    {
        int index = 0;
        foreach(var item in currentItems)
        {
            InventorySlot slot = inventorySlots[index];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if(itemInSlot == null)
            {
                Sprite incomingSprite = Resources.Load<Sprite>($"Assets/Game UI/Items/{item.name.ToLower()}");
                item.image = incomingSprite;

                SpawnNewItem(item, slot);
            }
            index++;

            if (index >= inventorySlots.Length)
            {
                break;
            }
        }
    }
}
