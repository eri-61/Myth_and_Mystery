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
    public Button inventoryBtn;
    public Button menuBtn;
    public Button journalBtn;
    public Button mapBtn;

    [Header("Panels")]
    public GameObject inventoryPanel;
    public GameObject menuPanel; 
    public GameObject instructionsPanel;
    public GameObject mapPanel;
    public GameObject uiPanel;

    [Header("Close  Panels Button")]
    public Button closeInv;
    public Button closeMap;
    public Button closeMenu;
    public Button closeInstructions;

    [Header("Variables")]
    bool isUIHidden = false;
    [SerializeField] private cherrydev.DialogBehaviour dialogBehaviour;

    private void Awake()
    {
        
    }
    #endregion

    void OnEnable()
    {
        skipBtn.onClick.AddListener(Skip);
        hideBtn.onClick.AddListener(HideUI);        
        
        inventoryBtn.onClick.AddListener(OpenInventory);
        menuBtn.onClick.AddListener(OpenMenu);
        journalBtn.onClick.AddListener(OpenJournal);
        mapBtn.onClick.AddListener(OpenMap);
        
        closeInv.onClick.AddListener(CloseInv);
        closeMap.onClick.AddListener(CloseMap);
        closeMenu.onClick.AddListener(CloseMenu);
    }

    void OnDisable()
    {
        skipBtn.onClick.RemoveListener(Skip);
        hideBtn.onClick.RemoveListener(HideUI);
        
        inventoryBtn.onClick.RemoveListener(OpenInventory);
        menuBtn.onClick.RemoveListener(OpenMenu);
        journalBtn.onClick.RemoveListener(OpenJournal);
        mapBtn.onClick.RemoveListener(OpenMap);
        
        closeInv.onClick.RemoveListener(CloseInv);
        closeMap.onClick.RemoveListener(CloseMap);
        closeMenu.onClick.RemoveListener(CloseMenu);
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
        if (dialogBehaviour != null)
        {
            dialogBehaviour.SkipCurrentSentence();
            dialogBehaviour.SkipToNextAnswerNode();
        }
    }

    void HideUI()
    {
        ToggleUI();
        uiPanel.SetActive(!isUIHidden);
        skipBtn.gameObject.SetActive(!isUIHidden);
    }

    void OpenInventory()
    {
        inventoryPanel.SetActive(true);
    }

    void OpenMenu()
    {
        menuPanel.SetActive(true);
    }

    void OpenJournal()
    {
        int journalIndex = GameManager.Instance.GetJournalSceneIndex();
        SceneController.Instance.LoadAdditiveScene(journalIndex);
    }

    void OpenMap()
    {
        MapManager.Instance.ShowMap();   
    }

    void CloseInv()
    {
        inventoryPanel.SetActive(false);
    }

    void CloseMap()
    {
        mapPanel.SetActive(false);
    }

    void CloseMenu()
    {
        menuPanel.SetActive(false);
    }

}
