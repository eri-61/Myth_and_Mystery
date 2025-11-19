using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JournalTabManager : MonoBehaviour
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
        JournalManager.instance.csScript.UpdateCaseFileUI();
    }

    void openClues()
    {
        ShowTab(cluesTab);
        JournalManager.instance.cluesScript.UpdateClueUI();
    }

    void openSL()
    {
        ShowTab(saveLoadTab);
        JournalManager.instance.slScript.UpdateSaveLoadUI();
    }

    void openCreatures()
    {
        ShowTab(creaturesTab);
        JournalManager.instance.creaturesScript.UpdateCreaturesUI();
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
        Time.timeScale = 1f;
        DialogController.instance.EnableDialogInput();
        Journal.SetActive(false);
    }
}

