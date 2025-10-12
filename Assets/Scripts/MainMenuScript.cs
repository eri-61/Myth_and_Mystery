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

    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject loadPanel;
    public GameObject quitPanel;

    [Header("Variables")]
    public int sceneIndex = 1;
    #endregion

    void OnEnable()
    {
        startBtn.onClick.AddListener(StartNewGame);
        loadBtn.onClick.AddListener(LoadGame);
        loadChapterBtn.onClick.AddListener(LoadChapter);
        journalBtn.onClick.AddListener(OpenJournal);
        settingsBtn.onClick.AddListener(OpenSettings);
        exitBtn.onClick.AddListener(ExitGame);
        yesBtn.onClick.AddListener(() => Application.Quit());
        noBtn.onClick.AddListener(() => quitPanel.SetActive(false));
    }

    void OnDisable()
    {
        startBtn.onClick.RemoveListener(StartNewGame);
        loadBtn.onClick.RemoveListener(LoadGame);
        loadChapterBtn.onClick.RemoveListener(LoadChapter);
        journalBtn.onClick.RemoveListener(OpenJournal);
        settingsBtn.onClick.RemoveListener(OpenSettings);
        exitBtn.onClick.RemoveListener(ExitGame);
        yesBtn.onClick.RemoveListener(() => Application.Quit());
        noBtn.onClick.RemoveListener(() => quitPanel.SetActive(false));
    }

    void StartNewGame()
    {
        GameManager.Instance.StartNewGame();
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
        SceneManager.LoadScene(2);
    }

    void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    void ExitGame()
    {
        quitPanel.SetActive(true);
    }
}
