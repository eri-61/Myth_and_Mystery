using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Variables")]
    public bool isNewGame = false;
    public int currentChapter = 1;
    public bool oldCaseFile = false;

    [Header("Journal")]
    public CaseFileScript cfScript;
    public CluesScript cluesScript;
    public CreaturesScript creaturesScript;
    public SaveLoadScript slScript;

    [Header("Chapter Items")]
    public List<ItemData> chapter1Items;
    public List<ItemData> chapter2Items;
    public List<ItemData> chapter3Items;
    public List<ItemData> chapter4Items;

    [Header("Dialog")]
    private DialogNodeBasedSystem.Scripts.DialogStarter dialog;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void RegisterDialog(DialogNodeBasedSystem.Scripts.DialogStarter dialogStarter)
    {
        dialog = dialogStarter;
    }

    public void WaitForJournal()
    {
        int index = GetJournalSceneIndex();
        SceneController.Instance.LoadAdditiveScene(index);
    }

    public void JournalClosed()
    {
        int index = GetJournalSceneIndex();
        if (SceneManager.GetSceneByBuildIndex(index).isLoaded)
        {
            SceneController.Instance.UnloadScene(index);
        }

        Time.timeScale = 1f;

        if (dialog != null)
            dialog.OnJournalClosed();
    }

    public int GetJournalSceneIndex()
    {
        switch (currentChapter)
        {
            case 1: return 4;
            case 2: return 5;
            default: return 0;
        }
    }

    public void StartNewGame()
    {
        isNewGame = true;
        currentChapter = 1;
        SceneManager.LoadScene(5);
    }

    public void LoadChapter(int index)
    {
        isNewGame = false;
        SceneManager.LoadScene(index);
    }

    public void LoadSave()
    {
        isNewGame = false;

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.LoadChapterItems(GetCurrentChapterItems());
        }
    }

    public void RegisterCaseFile(CaseFileScript caseFile)
    {
        cfScript = caseFile;
    }

    public void RegisterClues(CluesScript clues)
    {
        cluesScript = clues;
    }

    public void RegisterCreatures(CreaturesScript creatures)
    {
        creaturesScript = creatures;
    }

    public void RegisterSaveLoad(SaveLoadScript sl)
    {
        slScript = sl;
    }

    // Inventory operations
    public List<ItemData> GetCurrentChapterItems()
    {
        switch (currentChapter)
        {
            case 1: return chapter1Items;
            case 2: return chapter2Items;
            case 3: return chapter3Items;
            case 4: return chapter4Items;
            default: return new List<ItemData>();
        }
    }

    public void AddItem(ItemData newItem)
    {
        List<ItemData> chapterItems = GetCurrentChapterItems();

        if (!chapterItems.Contains(newItem))
        {
            chapterItems.Add(newItem);
            newItem.inInventory = true;

            if (InventoryManager.Instance != null)
                InventoryManager.Instance.LoadChapterItems(chapterItems);
        }
    }

    public void RemoveItem(ItemData item)
    {
        List<ItemData> chapterItems = GetCurrentChapterItems();

        if (chapterItems.Contains(item))
        {
            chapterItems.Remove(item);
            item.inInventory = false;

            if (InventoryManager.Instance != null)
                InventoryManager.Instance.LoadChapterItems(chapterItems);
        }
    }
}
