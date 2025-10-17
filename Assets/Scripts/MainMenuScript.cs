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
    public GameObject settingsPanel;
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
        SceneController.Instance.LoadScene(newGameSceneIndex);
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
        settingsPanel.SetActive(true);
    }

    void ExitGame()
    {
        quitPanel.SetActive(true);
    }

    //load panels
    void LoadChapter1()
    {
        SceneController.Instance.LoadScene(chapter1SceneIndex);
    }

    void LoadChapter2()
    {
        SceneController.Instance.LoadScene(chapter2SceneIndex);
    }

    void LoadChapter3()
    {
        SceneController.Instance.LoadScene(chapter3SceneIndex);
    }

    void LoadChapter4()
    {
        SceneController.Instance.LoadScene(chapter4SceneIndex);
    }

    void CloseLoad()
    {
        loadPanel.SetActive(false);
    }
}
