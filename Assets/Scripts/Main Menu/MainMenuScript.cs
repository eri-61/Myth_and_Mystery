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

    [Header("LoadPanel")]
    public Button closeLoadPanel;
    public Button loadChap1;
    public Button loadChap2;
    public Button loadChap3;
    public Button loadChap4;

    [Header("Panels")]
    public GameObject loadPanel;
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
        if (SettingsScript.instance == null)
        {
            //SettingsScript.instance.CloseSettings();
        }
        loadPanel.SetActive(false);
        quitPanel.SetActive(false);
        loadChap1.interactable = false;
        loadChap2.interactable = false;
        loadChap3.interactable = false;
        loadChap4.interactable = false;



        if(SaveManager.Instance == null)
        {
            SaveManager sm = new SaveManager();
        }
        
        SaveManager.Instance.InitGameStates();

        switch (SaveManager.Instance.CurrentGameState.PlayerSaves.Count)
        {
            case 1:
                loadChap1.interactable = true;
                break;
            case 2:
                loadChap1.interactable = true;
                loadChap2.interactable = true;
                break;
            case 3:
                loadChap1.interactable = true;
                loadChap2.interactable = true;
                loadChap3.interactable = true;
                break;
            case 4:
                loadChap1.interactable = true;
                loadChap2.interactable = true;
                loadChap3.interactable = true;
                loadChap4.interactable = true;
                break;
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

        //load chapters
        closeLoadPanel.onClick.AddListener(CloseLoad);
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

        //load chapters
        closeLoadPanel.onClick.RemoveListener(CloseLoad);
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
        // Load from save file
        loadPanel.SetActive(true);
    }

    void LoadChapter()
    {
        // Load a chapter
        loadPanel.SetActive(true);
    }

    void OpenJournal()
    {
        SceneManager.LoadScene(achievementSceneIndex);
    }

    void OpenSettings()
    {
        SettingsScript.instance.OpenSettings();
    }

    void ExitGame()
    {
        quitPanel.SetActive(true);
    }

    //load panels
    void LoadChapter1()
    {
        //temp Save 1
        SaveManager.Instance.LoadGameState(0);
        SaveManager.Instance.PrintGameStatus();

        //Load Chapter
        //if Chapter 1 Dialog 2, then load "VN_OfficeD2"
        SceneManager.LoadScene("VN_Office");

        //SceneManager.LoadScene(chapter1SceneIndex);
    }

    void LoadChapter2()
    {
        //temp Save 2
        SaveManager.Instance.LoadGameState(1);
        SaveManager.Instance.PrintGameStatus();

        //Load Chapter
        //if Chapter 1 Dialog 2, then load "VN_OfficeD2"
        SceneManager.LoadScene("VN_OfficeD2");

        //SceneManager.LoadScene(chapter2SceneIndex);
    }

    void LoadChapter3()
    {
        SceneManager.LoadScene(chapter3SceneIndex);
    }

    void LoadChapter4()
    {
        SceneManager.LoadScene(chapter4SceneIndex);
    }

    void CloseLoad()
    {
        loadPanel.SetActive(false);
    }
}
