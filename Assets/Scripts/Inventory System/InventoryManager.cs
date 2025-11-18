using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    #region Variables
    public static InventoryManager instance { private set; get; }
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;

    int selectedSlot = -1;
    [HideInInspector] public List<Items> currentItems = new List<Items>();
    #endregion

    void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

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
                    InventoryManager.instance.AddItem(loadedItem);
                }
            }
        }
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
        if(selectedSlot >= 0)
        {
            inventorySlots[selectedSlot].Deselect();
        }
        selectedSlot = itemSlot;
        inventorySlots[selectedSlot].Select();
    }

    public void AddItem(Items item)
    {
        for(int i = 0; i< inventorySlots.Length; i++)
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

    public Items GetSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if(itemInSlot != null)
        {
            Items item = itemInSlot.item;
            if (use == true)
            {
                itemInSlot.Count--;
                if (itemInSlot.Count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                }
            }
            return item;
        }
        return null;
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
                SpawnNewItem(item, slot);
            }
            index++;
        }
    }
}
