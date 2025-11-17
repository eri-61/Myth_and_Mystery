using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class VisualNovelScript : MonoBehaviour
{
    #region Variables

    [Header("Visual Novel Buttons")]
    public Button skipBtn;
    public Button hideBtn;

    [Header("Visual Novel Button - Image")]
    public Image hsButton;
    public Sprite hideSprite;
    public Sprite showSprite;

    [Header("Other Buttons")]
    public GameObject buttons;
    public Button inventoryBtn;
    public Button menuBtn;
    public Button journalBtn;
    public Button mapBtn;

    [Header("Panels")]
    public GameObject instructionsPanel;
    public GameObject settingsPanel;
    public GameObject inventoryPanel;
    public GameObject journalPanel;
    public GameObject mapPanel;
    public GameObject uiPanel;

    [Header("Close  Panels Button")]
    public Button closeInstructions;

    [Header("Variables")]
    bool isUIHidden = false;
    bool skipAllMode = false;
    #endregion

    private void Start()
    {
        skipBtn.onClick.AddListener(Skip);
        hideBtn.onClick.AddListener(HideUI);

        inventoryBtn.onClick.AddListener(OpenInventory);
        menuBtn.onClick.AddListener(OpenMenu);
        journalBtn.onClick.AddListener(OpenJournal);
        mapBtn.onClick.AddListener(OpenMap);
    }

    void OnDisable()
    {
        skipBtn.onClick.RemoveListener(Skip);
        hideBtn.onClick.RemoveListener(HideUI);

        inventoryBtn.onClick.RemoveListener(OpenInventory);
        menuBtn.onClick.RemoveListener(OpenMenu);
        journalBtn.onClick.RemoveListener(OpenJournal);
        mapBtn.onClick.RemoveListener(OpenMap);
      
    }

    void ToggleUI()
    {
        isUIHidden = !isUIHidden;

        if(hsButton != null)
        {
            hsButton.sprite = isUIHidden ? showSprite : hideSprite;
        }
    }

    void Skip()
    {
        skipAllMode = !skipAllMode;
        if (DialogController.instance != null)
            DialogController.instance.ToggleSkipAll(skipAllMode);
    }

    void HideUI()
    {
        ToggleUI();
        uiPanel.SetActive(!isUIHidden);
        buttons.SetActive(!isUIHidden);
        skipBtn.gameObject.SetActive(!isUIHidden);
    }

    void OpenInventory()
    {
        inventoryPanel.SetActive(true);
    }

    void OpenMenu()
    {
       settingsPanel.SetActive(true);
    }

    void OpenJournal()
    {
        journalPanel.SetActive(true);
    }

    void OpenMap()
    {
        mapPanel.SetActive(true);
    }

}
