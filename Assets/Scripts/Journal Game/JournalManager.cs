using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class JournalManager : MonoBehaviour
{
    #region Journal Variables
    public GameObject Journal;

    [Header("Journal Buttons")]
    public Button caseButton;
    public Button cluesBtn;
    public Button slBtn;
    public Button creaturesBtn;

    [Header("Tabs")]
    public GameObject caseFileTab;
    public GameObject cluesTab;
    public GameObject creaturesTab;
    public GameObject saveLoadTab;

    [Header("Close Button")]
    public Button closeBtn;

    [Header("Scripts")]
    public CaseFileScript csScript;
    public CluesScript cluesScript;
    public CreaturesScript creaturesScript;
    public SaveLoadScript slScript;
    #endregion

    void OnEnable()
    {
        caseButton.onClick.AddListener(OpenCaseFile);
        cluesBtn.onClick.AddListener(openClues);
        slBtn.onClick.AddListener(openSL);
        creaturesBtn.onClick.AddListener(openCreatures);
        closeBtn.onClick.AddListener(CloseTab);
        Time.timeScale = 0f;
    }

    void OnDisable()
    {
        caseButton.onClick.RemoveListener(OpenCaseFile);
        cluesBtn.onClick.RemoveListener(openClues);
        slBtn.onClick.RemoveListener(openSL);
        creaturesBtn.onClick.RemoveListener(openCreatures);
        closeBtn.onClick.RemoveListener(CloseTab);
    }

    //open tabs
    void OpenCaseFile()
    {
        ShowTab(caseFileTab);
        csScript.UpdateCaseFileUI();
    }

    void openClues()
    {
        ShowTab(cluesTab);
        //cluesScript.UpdateCluesUI();
    }

    void openSL()
    {
        //Saving Game here

        var curGS = SaveManager.Instance.GetGameState();
        if (curGS != null) 
        {
            curGS.LastSaveDateTime = DateTime.Now;
            SaveManager.Instance.SaveGameState(SaveManager.Instance.SelectedSaveIndex);

            SaveManager.Instance.PrintGameStatus();
        }

        ShowTab(saveLoadTab);
        //slScript.UpdateSLUI();
    }

    void openCreatures()
    {
        ShowTab(creaturesTab);
        //creaturesScript.UpdateCreaturesUI();
    }

    private void ShowTab(GameObject activeTab)
    {
        caseFileTab.SetActive(false);
        cluesTab.SetActive(false);
        creaturesTab.SetActive(false);
        saveLoadTab.SetActive(false);

        if (activeTab != null)
        {
            activeTab.SetActive(true);
        }
    }

    private void CloseTab()
    {
        Journal.SetActive(false);
    }
}

