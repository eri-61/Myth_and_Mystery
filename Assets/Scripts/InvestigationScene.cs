using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class InvestigationScene : MonoBehaviour
{
    [Header("Choice")]
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button talk;
    [SerializeField] private Button investigate;

    [Header("Main Buttons")]
    [SerializeField] private Button journalButton;
    [SerializeField] private Button settingsButton;

    [Header("Back Button")]
    [SerializeField] private Button backButton;
    [SerializeField] private GameObject back;

    [Header("Panels")]
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("UI Elements")]
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Settings")]
    public bool characterExists = true;
    public int talkSceneIndex = 2;
    public float dialogHide = 0.05f;

    [Header("Investigation Points")]
    public GameObject investigationButtons;
    public Button[] investigationPoints;
    [TextArea] public string[] textDescriptions;
    public int tentScene = 1;

    [Header("Items & Clues (Match by Index)")]
    public ItemData[] items;
    public CluesData[] clues;

    [Header("Items & Clues script")]
    [SerializeField] private CluesScript cluScript;
    [SerializeField] private OLD_InventoryManager inventoryManager;

    private Dictionary<int, bool> isPointSearched = new Dictionary<int, bool>();
    private Coroutine hideDialogCoroutine;

    void Start()
    {
        Debug.Log($"InvestigationScene > Start");
        buttons.SetActive(true);
        back.SetActive(false);
        investigationButtons.SetActive(false);

        description.text = "Objective: Investigate the area to find clues.";
        dialogPanel.SetActive(true);

        character.SetActive(characterExists);

        for (int i = 0; i < investigationPoints.Length; i++)
        {
            isPointSearched[i] = false;
        }

        inventoryManager = inventoryPanel.GetComponent<OLD_InventoryManager>();

        //Check Game Save and re-apply found items
        var curGS = SaveManager.Instance.GetGameState();
        if(curGS.Inventory != null)
        {
            if (curGS.Inventory.Items != null)
            {
                Debug.Log($"InvestigationScene > Reloading Inventory Items > Item Count: {curGS.Inventory.Items.Count}");
                foreach (var itm in curGS.Inventory.Items)
                {
                    Sprite incomingSprite = Resources.Load<Sprite>($"Assets/Game UI/Items/{itm.ItemName.ToLower().Replace("data", "")}_0");

                    //inventoryManager.currentItems.Add
                    //AddToInventory(itm.ItemIndex,new ItemData()
                    //{
                    //    itemName = itm.ItemName,
                    //    itemDescription = itm.ItemDescription,
                    //    itemSprite = incomingSprite
                    //});
                    inventoryManager.currentItems.Add(new ItemData()
                    {
                        itemName = itm.ItemName,
                        itemDescription = itm.ItemDescription,
                        itemSprite = incomingSprite
                    });
                    //AddToInventory(itm.ItemIndex, items[itm.ItemIndex]);
                }
            }
        }

        if (curGS.Journal != null)
        {
            if (curGS.Journal.Clues != null)
            {
                Debug.Log($"InvestigationScene > Reloading Journal Clues > Clues Count: {curGS.Journal.Clues.Count}");
                foreach (var itm in curGS.Journal.Clues)
                {
                    string spritePath = $"Assets/Game UI/Clues and Testimony/{itm.ClueName.ToLower().Replace("data", "")}_0";
                    Sprite incomingSprite = Resources.Load<Sprite>(spritePath);

                    //AddClueToJournal(itm.ClueNumber, new CluesData()
                    //{
                    //    clueName = itm.ClueName,
                    //    clueDescription = itm.ClueDescription,
                    //    clueImage = incomingSprite
                    //});
                    cluScript.gatheredClues.Add(new CluesData()
                    {
                        clueName = itm.ClueName,
                        clueDescription = itm.ClueDescription,
                        clueImage = incomingSprite
                    });
                    //AddClueToJournal(itm.ClueNumber, clues[itm.ClueNumber]);
                }
            }
        }

    }

    void OnEnable()
    {
        Debug.Log($"InvestigationScene > Enable");
        for (int i = 0; i < investigationPoints.Length; i++)
        {
            int index = i;
            investigationPoints[i].onClick.AddListener(() => ClickInvestigationPoint(index));
        }

        backButton.onClick.AddListener(GoBack);
        talk.onClick.AddListener(TalkToCharacter);
        investigate.onClick.AddListener(async ()=> { await StartInvestigation(); });

        journalButton.onClick.AddListener(OpenJournal);
        settingsButton.onClick.AddListener(OpenMenu);
    }

    void OnDisable()
    {
        Debug.Log($"InvestigationScene > Disable");
        foreach (Button point in investigationPoints)
            point.onClick.RemoveAllListeners();

        backButton.onClick.RemoveAllListeners();
        talk.onClick.RemoveAllListeners();
        investigate.onClick.RemoveAllListeners();

        journalButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
    }

    void TalkToCharacter()
    {
        if (!characterExists)
        {
            ShowDialog("There’s no one to talk to here.");
        }
        else
        {
            SceneManager.LoadScene(talkSceneIndex);
        }
    }

    void GoBack()
    {
        back.SetActive(false);
        investigationButtons.SetActive(false);

        buttons.SetActive(true);
        character.SetActive(characterExists);

    }

    async Task StartInvestigation()
    {
        await Task.Delay(900);
        buttons.SetActive(false);
        character.SetActive(false);

        back.SetActive(true);
        investigationButtons.SetActive(true);

        ShowDialog("Click on the areas you want to investigate.");

    }

    void ClickInvestigationPoint(int index)
    {
        Debug.Log($"InvestigationScene > ClickInvestigationPoint");

        if (isPointSearched[index])
        {
            ShowDialog("You’ve already searched this area.");
            return;
        }

        if (index < textDescriptions.Length)
            ShowDialog(textDescriptions[index]);
        else
            ShowDialog("There’s nothing of interest here.");

        if (index < clues.Length && clues[index] != null)
        {
            AddClueToJournal(index,clues[index]);
        }

        if (index < items.Length && items[index] != null)
        {
            AddToInventory(index,items[index]);
        }

        isPointSearched[index] = true;
        //StartCoroutine(HideDialog(dialogHide));
        HideDialogAsync();
    }

    void ShowDialog(string message)
    {
        Debug.Log($"InvestigationScene > ShowDialog");
        dialogPanel.SetActive(true);
        description.text = message;

        if (hideDialogCoroutine != null)
        {
            StopCoroutine(hideDialogCoroutine);
            hideDialogCoroutine = null;
        }

        //hideDialogCoroutine = StartCoroutine(HideDialog(dialogHide));
        HideDialogAsync();
    }

    IEnumerator HideDialog(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogPanel.SetActive(false);
        hideDialogCoroutine = null;
    }

    async Task HideDialogAsync()
    {
        Debug.Log($"InvestigationScene > HideDialogAsync");
        await Task.Delay(900);
        dialogPanel.SetActive(false);
        hideDialogCoroutine = null;
    }

    void OpenMenu()
    {
        Debug.Log($"InvestigationScene > OpenInventory");
        settingsPanel.SetActive(true);
    }

    void OpenJournal()
    {
        Debug.Log($"InvestigationScene > OpenJournal");
        journalPanel.SetActive(true);
    }

    void OpenMap()
    {
        Debug.Log($"InvestigationScene > OpenMap");
        mapPanel.SetActive(true);
    }

    void AddClueToJournal(int index,CluesData clue)
    {
        Debug.Log($"InvestigationScene > AddClueToJournal");
        if (clue == null)
        {
            Debug.LogWarning("[InvestigationScene] AddClueToJournal called with null clue.");
            return;
        }

        if (cluScript != null)
        {
            CluesScript.Instance.AddClues(clue);

            Debug.Log($"InvestigationScene > AddClueToJournal > Add Clue to Game Save Memory");
            var curGS = SaveManager.Instance.GetGameState();
            if (curGS != null)
            {
                if (curGS.Journal == null)
                {
                    curGS.Journal = new GameJournal();
                }

                if (curGS.Journal.Clues == null)
                {
                    curGS.Journal.Clues = new List<JournalClue>();
                }

                var existingClue = curGS.Journal.Clues.Where(c => c.ClueName == clue.name).FirstOrDefault();

                if(existingClue == null && !string.IsNullOrWhiteSpace(clue.name))
                {
                    curGS.Journal.Clues.Add(new JournalClue()
                    {
                        ClueNumber = index,
                        ClueName = clue.name,
                        ClueDescription = clue.clueDescription
                    });
                }
                
            }

            Debug.Log($"[InvestigationScene] Added clue: {clue.clueName}");
        }
        else
        {
            Debug.LogWarning("[InvestigationScene] CluesScript reference is not set in the Inspector.");
        }
    }

    void AddToInventory(int index, ItemData newItem)
    {
        Debug.Log($"InvestigationScene > AddToInventory");
        if (newItem == null)
        {
            Debug.LogWarning("[InvestigationScene] AddToInventory called with null item.");
            return;
        }

        if (inventoryManager != null)
        {
            inventoryManager.AddItem(index,newItem);
            Debug.Log($"[InvestigationScene] Added item via serialized ref: {newItem.itemName}");
        }
    }

    public void GoToScene()
    {
        Debug.Log($"InvestigationScene > GoToScene");
        SceneManager.LoadScene(tentScene);
    }
    
}