using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public BattleSystem battleSystem;

    private List<ItemData> currentItems = new List<ItemData>();
    [HideInInspector] public ItemData selectedItem;
    #endregion

    private void Awake()
    {
        
    }

    private void Start()
    {
        close.onClick.AddListener(() => gameObject.SetActive(false));
        useItem.onClick.AddListener(OnUseItem);
    }

    public void LoadChapterItems(List<ItemData> items)
    {
        currentItems.Clear();

        // Only include items that are inInventory
        foreach (var item in items)
        {
            if (item.inInventory)
                currentItems.Add(item);
        }

        LoadInventoryUI();
    }

    public void LoadInventoryUI()
    {
        int idx = 1;

        //slots1 = InventoryManager.Instance.;
        //slots2 = InventoryManager.Instance.slots2;
        //slots3 = InventoryManager.Instance.slots3;
        //slots4 = InventoryManager.Instance.slots4;
        //slots5 = InventoryManager.Instance.slots5;
        //slots6 = InventoryManager.Instance.slots6;
        //slots7 = InventoryManager.Instance.slots7;
        //slots8 = InventoryManager.Instance.slots8;
        //slots9 = InventoryManager.Instance.slots9;

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

    private void OnUseItem()
    {
        if (selectedItem == null || battleSystem == null) return;
        if (battleSystem.state != Battlestate.PLAYERTURN) return;

        if (selectedItem == battleSystem.correctItem)
        {
            battleSystem.dialogueText.text = $"You used {selectedItem.itemName}! It was super effective!";
            battleSystem.enemyUnit.currentHP = 1;
            battleSystem.enemyHUD.setHP(1);
        }
        else
        {
            battleSystem.dialogueText.text = $"You used {selectedItem.itemName}, but it had no effect!";
            battleSystem.nextEnemyAttackDoubles = true;
        }

        gameObject.SetActive(false);
        battleSystem.StartCoroutine(AfterUseItem());
    }

    private System.Collections.IEnumerator AfterUseItem()
    {
        yield return new WaitForSeconds(2f);
        battleSystem.state = Battlestate.ENEMYTURN;
        battleSystem.StartCoroutine(battleSystem.EnemyTurn());
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

}
