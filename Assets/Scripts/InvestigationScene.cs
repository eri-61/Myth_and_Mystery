using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;

public class InvestigationScene : MonoBehaviour
{
    [Header("Choice")]
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button talk;
    [SerializeField] private Button investigate;

    [Header("Main Buttons")]
    [SerializeField] private Button mapButton;
    [SerializeField] private Button journalButton;
    [SerializeField] private Button inventoryButton;
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
    public float dialogHide = 0.5f;

    [Header("Investigation Points")]
    public GameObject investigationButtons;
    public Button[] investigationPoints;
    [TextArea] public string[] textDescriptions;

    [Header("Items & Clues (Match by Index)")]
    public ItemData[] items;
    public CluesData[] clues;

    [Header("Items & Clues script")]
    [SerializeField] private CluesScript CluesScript;
    [SerializeField] private InventoryManager inventoryManager;

    private Dictionary<int, bool> isPointSearched = new Dictionary<int, bool>();

    void Start()
    {
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
    }

    void OnEnable()
    {
        for (int i = 0; i < investigationPoints.Length; i++)
        {
            int index = i;
            investigationPoints[i].onClick.AddListener(() => ClickInvestigationPoint(index));
        }

        backButton.onClick.AddListener(GoBack);
        talk.onClick.AddListener(TalkToCharacter);
        investigate.onClick.AddListener(StartInvestigation);

        journalButton.onClick.AddListener(OpenJournal);
        mapButton.onClick.AddListener(OpenMap);
        settingsButton.onClick.AddListener(OpenMenu);
        inventoryButton.onClick.AddListener(OpenInventory);

    }

    void OnDisable()
    {
        foreach (Button point in investigationPoints)
            point.onClick.RemoveAllListeners();

        backButton.onClick.RemoveAllListeners();
        talk.onClick.RemoveAllListeners();
        investigate.onClick.RemoveAllListeners();

        journalButton.onClick.RemoveAllListeners();
        mapButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        inventoryButton.onClick.RemoveAllListeners();
    }

    void TalkToCharacter()
    {
        if (!characterExists)
        {
            ShowDialog("There’s no one to talk to here.");
        }
        else
        {
            SceneController.Instance.LoadScene(talkSceneIndex);
        }
    }

    void GoBack()
    {
        back.SetActive(false);
        investigationButtons.SetActive(false);

        buttons.SetActive(true);
        character.SetActive(characterExists);

        dialogPanel.SetActive(true);
        description.text = "Choose an options.";
    }

    void StartInvestigation()
    {
        buttons.SetActive(false);
        character.SetActive(false);

        back.SetActive(true);
        investigationButtons.SetActive(true);

        ShowDialog("Click on the areas you want to investigate.");
    }

    void ClickInvestigationPoint(int index)
    {
        dialogPanel.SetActive(true);

        if (isPointSearched[index])
        {
            ShowDialog("You’ve already searched this area.");
            return;
        }

        if (index < textDescriptions.Length)
            description.text = textDescriptions[index];
        else
            description.text = "There’s nothing of interest here.";

        if (index < clues.Length && clues[index] != null)
        {
            AddClueToJournal(clues[index]);
        }

        if (index < items.Length && items[index] != null)
        {
            AddToInventory(items[index]);
        }

        isPointSearched[index] = true;
        StartCoroutine(HideDialog(dialogHide));
    }

    void ShowDialog(string message)
    {
        description.text = message;
        dialogPanel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HideDialog(dialogHide));
    }

    IEnumerator HideDialog(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogPanel.SetActive(false);
    }

    void OpenInventory()
    {
        PersistentObjects.instance.OpenInventory();
    }

    void OpenMenu()
    {
        PersistentObjects.instance.OpenSettings();
    }

    void OpenJournal()
    {
        PersistentObjects.instance.OpenJournal();
    }

    void OpenMap()
    {
        PersistentObjects.instance.OpenMap();
    }

    void CloseInv()
    {
        PersistentObjects.instance.CloseInventory();
    }

    void CloseMap()
    {
        PersistentObjects.instance.CloseMap();
    }

    void CloseMenu()
    {
        PersistentObjects.instance.CloseSettings();
    }
    void AddClueToJournal(CluesData clue)
    {
        if (clue == null)
        {
            Debug.LogWarning("[InvestigationScene] AddClueToJournal called with null clue.");
            return;
        }

        if (CluesScript != null)
        {
            GameManager.Instance.cluesScript.AddClues(clue);
            Debug.Log($"[InvestigationScene] Added clue: {clue.clueName}");
        }
        else
        {
            Debug.LogWarning("[InvestigationScene] CluesScript reference is not set in the Inspector.");
        }
    }

    void AddToInventory(ItemData newItem)
    {
        if (newItem == null)
        {
            Debug.LogWarning("[InvestigationScene] AddToInventory called with null item.");
            return;
        }

        if (inventoryManager != null)
        {
            inventoryManager.AddItem(newItem);
            Debug.Log($"[InvestigationScene] Added item via serialized ref: {newItem.itemName}");
        }
        else if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(newItem);
            Debug.Log($"[InvestigationScene] Added item via InventoryManager.Instance: {newItem.itemName}");
        }
        else
        {
            Debug.LogWarning("[InvestigationScene] No InventoryManager found. Assign one in the Inspector or ensure InventoryManager.Instance exists.");
        }
    }
}