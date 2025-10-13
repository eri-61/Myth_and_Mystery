using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    #region Variables
    private List<InventoryData> currentItems = new List<InventoryData>();
    public int BattleSceneIndex = 2;

    [Header("Item Details")]
    public GameObject itemDetailsSection;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI Description;

    [Header("Inventory Slots")]
    public Button[] slots;

    [Header("Buttons")]
    public Button close;
    public Button useItem;

    [HideInInspector] public InventoryData selectedItem;
    public BattleSystem battleSystem;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int activeScene = SceneManager.GetActiveScene().buildIndex;
        if(activeScene == BattleSceneIndex)
        {
            battleSystem = FindObjectOfType<BattleSystem>();
        }

        useItem.onClick.AddListener(OnUseItem);
        close.onClick.AddListener(() => gameObject.SetActive(false));

        LoadInventory();
    }

    void OnDisable()
    {
        foreach (Button slot in slots)
        {
            slot.onClick.RemoveAllListeners();
        }
        close.onClick.RemoveListener(() => gameObject.SetActive(false));
    }

    public void LoadInventory()
    {
        currentItems = new List<InventoryData>(GameManager.Instance.inventory);

        for (int i = 0; i < slots.Length; i++)
        {
            Image slotImage = slots[i].GetComponent<Image>();
            slots[i].onClick.RemoveAllListeners();

            if (i < currentItems.Count)
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

        selectedItem = item;
    } 

    //battle
    public void OnUseItem()
    {
        if (selectedItem == null || battleSystem.state != Battlestate.PLAYERTURN)
            return;

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

        // Close inventory panel
        gameObject.SetActive(false);

        // Wait then go to enemy turn
        battleSystem.StartCoroutine(AfterUseItem());
    }

    private System.Collections.IEnumerator AfterUseItem()
    {
        yield return new WaitForSeconds(2f);
        battleSystem.state = Battlestate.ENEMYTURN;
        battleSystem.StartCoroutine(battleSystem.EnemyTurn());
    }

}
