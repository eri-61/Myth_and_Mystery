using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [Header("Main Menu Buttons")]
    public Button startBtn;
    public Button continueBtn;
    public Button loadBtn;
    public Button loadCheckpointBtn;
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

    void OnEnable()
    {
        startBtn.onClick.AddListener(StartNewGame);
        continueBtn.onClick.AddListener(ContinueGame);
        loadBtn.onClick.AddListener(LoadGame);
        loadCheckpointBtn.onClick.AddListener(LoadCheckpoint);
        journalBtn.onClick.AddListener(OpenJournal);
        settingsBtn.onClick.AddListener(OpenSettings);
        exitBtn.onClick.AddListener(ExitGame);
        yesBtn.onClick.AddListener(() => Application.Quit());
        noBtn.onClick.AddListener(() => quitPanel.SetActive(false));
    }

    void OnDisable()
    {
        startBtn.onClick.RemoveListener(StartNewGame);
        continueBtn.onClick.RemoveListener(ContinueGame);
        loadBtn.onClick.RemoveListener(LoadGame);
        loadCheckpointBtn.onClick.RemoveListener(LoadCheckpoint);
        journalBtn.onClick.RemoveListener(OpenJournal);
        settingsBtn.onClick.RemoveListener(OpenSettings);
        exitBtn.onClick.RemoveListener(ExitGame);
        yesBtn.onClick.RemoveListener(() => Application.Quit());
        noBtn.onClick.RemoveListener(() => quitPanel.SetActive(false));
    }

    void StartNewGame()
    {
        SceneManager.LoadScene(3);
    }

    void ContinueGame()
    {
        // Continue from autosave or last save point

    }

    void LoadGame()
    {
        // Load from save file
        loadPanel.SetActive(true);
    }

    void LoadCheckpoint()
    {
        // Load from checkpoint
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
