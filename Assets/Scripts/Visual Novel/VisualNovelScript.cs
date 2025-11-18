using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class VisualNovelScript : MonoBehaviour
{
    #region Variables

    [Header("Visual Novel Buttons")]
    public Button autoButton;
    public Button hideBtn;

    [Header ("AutoPlay Button")]
    public Image autoButtonImage;
    public Sprite autoPlaySprite;
    public Sprite pauseSprite;

    [Header("Visual Novel Button - Image")]
    public Image hsButton;
    public Sprite hideSprite;
    public Sprite showSprite;

    [Header("Other Buttons")]
    public GameObject buttons;
    public Button menuBtn;
    public Button journalBtn;

    [Header("Panels")]
    public GameObject instructionsPanel;
    public GameObject settingsPanel;
    public GameObject journalPanel;
    public GameObject uiPanel;

    [Header("Close  Panels Button")]
    public Button closeInstructions;

    [Header("Variables")]
    bool isUIHidden = false;
    bool skipAllMode = false;
    bool isAutoPlaying = false;
    #endregion

    private void Start()
    {
        autoButton.onClick.AddListener(Skip);
        hideBtn.onClick.AddListener(HideUI);

        menuBtn.onClick.AddListener(OpenMenu);
        journalBtn.onClick.AddListener(OpenJournal);
    }

    void OnDisable()
    {
        autoButton.onClick.RemoveListener(Skip);
        hideBtn.onClick.RemoveListener(HideUI);

        menuBtn.onClick.RemoveListener(OpenMenu);
        journalBtn.onClick.RemoveListener(OpenJournal);
      
    }

    void ToggleUI()
    {
        isUIHidden = !isUIHidden;

        if(hsButton != null)
        {
            hsButton.sprite = isUIHidden ? showSprite : hideSprite;
        }
    }
    void TogglePlayPause()
    {
        isAutoPlaying = !isAutoPlaying;

        if (autoButton != null)
        {
            autoButtonImage.sprite = isAutoPlaying ? pauseSprite : autoPlaySprite;
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
        autoButton.gameObject.SetActive(!isUIHidden);
    }

    void OpenMenu()
    {
       settingsPanel.SetActive(true);
    }

    void OpenJournal()
    {
        JournalManager.instance.OpenJournal();
    }

}
