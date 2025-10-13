using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    #region
    public static GameManager Instance;

    [Header ("Variables")]
    public bool isNewGame = false;
    public int currentChapter = 1;
    public bool waitingForJournal = false;
    public bool oldCaseFile = false;

    [Header("Journal")]
    public CaseFileScript cfScript;
    public CluesScript cluesScript;
    public CreaturesScript creaturesScript;
    public SaveLoadScript slScript;

    [Header ("Inventory")]
    public List<InventoryData> inventory = new List<InventoryData>();

    [Header("Dialog")]
    private DialogNodeBasedSystem.Scripts.DialogStarterWIInfo dialog;
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterDialog(DialogNodeBasedSystem.Scripts.DialogStarterWIInfo dialogStarter)
    {
        dialog = dialogStarter;
    }

    public void WaitForJournal()
    {
        waitingForJournal = true;
        int index = GetJournalSceneIndex();
        SceneController.Instance.LoadAdditiveScene(index);
    }

    public void JournalClosed()
    {
        if (waitingForJournal)
        {
            waitingForJournal = false;
            int index = GetJournalSceneIndex();
            SceneController.Instance.UnloadScene(index);
            dialog.OnJournalClosed();
        }
        else
        {
            SceneController.Instance.UnloadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1f;
        }
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
        //add code
    }

    //register journal items
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

    //inventory
    public void AddItem(InventoryData newItem)
    {
        if (!inventory.Contains(newItem))
        {
            inventory.Add(newItem);
            newItem.inInventory = true;
        }
    }

    public void RemoveItem(InventoryData item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            item.inInventory = false;
        }
    }
}
