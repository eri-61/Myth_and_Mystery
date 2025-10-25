using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    #region Variables
    [Header("Main Menu Buttons")]
    public Button startBtn;
    public Button loadBtn;
    public Button loadChapterBtn;
    public Button journalBtn;
    public Button settingsBtn;
    public Button exitBtn;

    [Header("Quit Confirmation Buttons")]
    public Button yesBtn;
    public Button noBtn;

    [Header("Load Save Slots")]
    public Button closeLoadSlotsPanel;
    public Button[] loadSlot;
    //public Button loadSlot1;
    //public Button loadSlot2;
    //public Button loadSlot3;
    //public Button loadSlot4;
    //public Button loadSlot5;
    //public Button loadSlot6;
    //public Button loadSlot7;
    //public Button loadSlot8;

    [Header("Lod  Chapters Panel")]
    public Button closeLoadPanel;
    public Button loadChap1;
    public Button loadChap2;
    public Button loadChap3;
    public Button loadChap4;

    [Header("Panels")]
    public GameObject loadPanel;
    public GameObject loadSlotsPanel;    
    public GameObject settingsPanel;
    public GameObject quitPanel;

    [Header("Variables")]
    public int newGameSceneIndex = 1;
    public int chapter1SceneIndex = 1;
    public int chapter2SceneIndex = 1;
    public int chapter3SceneIndex = 1;
    public int chapter4SceneIndex = 1;

    public int achievementSceneIndex = 1;
    #endregion

    private void Start()
    {
        loadPanel.SetActive(false);
        quitPanel.SetActive(false);

        loadChap1.interactable = true;
        loadChap2.interactable = false;
        loadChap3.interactable = false;
        loadChap4.interactable = false;

        for (int i = 0; i < loadSlot.Length; i++)
        {
            int index = i;
            loadSlot[index].interactable = false;
        }
        if (SaveManager.Instance == null)
        {
            SaveManager sm = new SaveManager();
        }
        
        SaveManager.Instance.InitGameStates();

        for(int i = 0; i < SaveManager.Instance.CurrentGameState.PlayerSaves.Count; i++)
        {
            int index = i;
            loadSlot[index].interactable = true;
        }
    }

    void OnEnable()
    {
        //main
        startBtn.onClick.AddListener(StartNewGame);
        loadBtn.onClick.AddListener(LoadGame);
        loadChapterBtn.onClick.AddListener(LoadChapter);
        journalBtn.onClick.AddListener(OpenJournal);
        settingsBtn.onClick.AddListener(OpenSettings);
        exitBtn.onClick.AddListener(ExitGame);

        //load slots
        for(int i =0; i < loadSlot.Length; i++)
        {
            int index = i; // Capture the current value of i
            loadSlot[i].onClick.AddListener(() => 
            {
                SaveManager.Instance.LoadGameState(index);
                SaveManager.Instance.PrintGameStatus();
                int curChap = SaveManager.Instance.CurrentGameSave.CurrentChapter - 1;
                int curDia = SaveManager.Instance.CurrentGameSave.CurrentDialog - 1;
                string incomingScene = GameConstants.ChapterDialogs[curChap, curDia];
                SceneManager.LoadScene(incomingScene);
            });
        }
        closeLoadSlotsPanel.onClick.AddListener(() => loadSlotsPanel.SetActive(false));

        //load chapters
        closeLoadPanel.onClick.AddListener(() => loadPanel.SetActive(false));
        loadChap1.onClick.AddListener(LoadChapter1);
        loadChap2.onClick.AddListener(LoadChapter2);
        loadChap3.onClick.AddListener(LoadChapter3);
        loadChap4.onClick.AddListener(LoadChapter4);

        //quit
        yesBtn.onClick.AddListener(() => Application.Quit());
        noBtn.onClick.AddListener(() => quitPanel.SetActive(false));
    }

    void OnDisable()
    {
        //main
        startBtn.onClick.RemoveListener(StartNewGame);
        loadBtn.onClick.RemoveListener(LoadGame);
        loadChapterBtn.onClick.RemoveListener(LoadChapter);
        journalBtn.onClick.RemoveListener(OpenJournal);
        settingsBtn.onClick.RemoveListener(OpenSettings);
        exitBtn.onClick.RemoveListener(ExitGame);

        //load slots
        for (int i = 0; i < loadSlot.Length; i++)
        { 
            int index = i; // Capture the current value of i
            loadSlot[i].onClick.RemoveListener(() => 
            {
                SaveManager.Instance.LoadGameState(index);
                SaveManager.Instance.PrintGameStatus();
                SceneManager.LoadScene(SaveManager.Instance.CurrentGameState.PlayerSaves[index].CurrentDialog);
            });
        }
        closeLoadSlotsPanel.onClick.RemoveListener(() => loadSlotsPanel.SetActive(false));

        //load chapters
        closeLoadPanel.onClick.RemoveListener(() => loadPanel.SetActive(false));
        loadChap1.onClick.RemoveListener(LoadChapter1);
        loadChap2.onClick.RemoveListener(LoadChapter2);
        loadChap3.onClick.RemoveListener(LoadChapter3);
        loadChap4.onClick.RemoveListener(LoadChapter4);

        //quit
        yesBtn.onClick.RemoveListener(() => Application.Quit());
        noBtn.onClick.RemoveListener(() => quitPanel.SetActive(false));
    }

    void StartNewGame()
    {
        int svIndex = SaveManager.Instance.NewGameState();

        SaveManager.Instance.PrintGameStatus();

        SceneManager.LoadScene(newGameSceneIndex);
    }

    void LoadGame()
    {
        loadSlotsPanel.SetActive(true);
    }

    void LoadChapter()
    {
        loadPanel.SetActive(true);
    }

    void OpenJournal()
    {
        SceneManager.LoadScene(achievementSceneIndex);
    }

    void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    void ExitGame()
    {
        quitPanel.SetActive(true);
    }

    //load panels
    void LoadChapter1()
    {
        SceneManager.LoadScene(chapter1SceneIndex);
    }

    void LoadChapter2()
    {
        SceneManager.LoadScene(chapter2SceneIndex);
    }

    void LoadChapter3()
    {
        SceneManager.LoadScene(chapter3SceneIndex);
    }

    void LoadChapter4()
    {
        SceneManager.LoadScene(chapter4SceneIndex);
    }
}
