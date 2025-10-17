using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    #region Variables
    public static InventoryManager Instance;

    [Header("item detals")]
    public GameObject itemDetailsSection;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI Description;

    [Header("ui")]
    public Button[] slots;
    public Button close;
    public Button useItem;
    public BattleSystem battleSystem;

    private List<ItemData> currentItems = new List<ItemData>();
    [HideInInspector] public ItemData selectedItem;
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
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

    private void LoadInventoryUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Image slotImage = slots[i].GetComponent<Image>();
            slots[i].onClick.RemoveAllListeners();

            if (i < currentItems.Count)
            {
                ItemData item = currentItems[i];
                slotImage.sprite = item.itemSprite;
                slots[i].onClick.AddListener(() => ShowItemDetails(item));
            }
            else slotImage.sprite = null;
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
