using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VisualNovelScript : MonoBehaviour
{
    [Header("Visual Novel Buttons")]
    public Button autoBtn;
    public Button speedBtn;
    public Button hideBtn;

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

    void onEnable()
    {
        autoBtn.onClick.AddListener(AutoPlay);
        speedBtn.onClick.AddListener(ChangeSpeed);
        hideBtn.onClick.AddListener(HideUI);        
        inventoryBtn.onClick.AddListener(OpenInventory);
        menuBtn.onClick.AddListener(OpenMenu);
        journalBtn.onClick.AddListener(OpenJournal);
        mapBtn.onClick.AddListener(OpenMap);
        closeInv.onClick.AddListener(CloseInv);
        closeMap.onClick.AddListener(CloseMap);
        closeMenu.onClick.AddListener(CloseMenu);
        closeInstructions.onClick.AddListener(CloseInstructions);
    }

    void onDisable()
    {
        autoBtn.onClick.RemoveListener(AutoPlay);
        speedBtn.onClick.RemoveListener(ChangeSpeed);
        hideBtn.onClick.RemoveListener(HideUI);
        inventoryBtn.onClick.RemoveListener(OpenInventory);
        menuBtn.onClick.RemoveListener(OpenMenu);
        journalBtn.onClick.RemoveListener(OpenJournal);
        mapBtn.onClick.RemoveListener(OpenMap);
        closeInv.onClick.RemoveListener(CloseInv);
        closeMap.onClick.RemoveListener(CloseMap);
        closeMenu.onClick.RemoveListener(CloseMenu);
        closeInstructions.onClick.RemoveListener(CloseInstructions);
    }

    void AutoPlay()
    {
        // Implement auto play functionality
    }

    void ChangeSpeed()
    {
        // Implement change speed functionality
    }

    void HideUI()
    {
        uiPanel.SetActive(false);
    }

    void OpenInstructions()
    {
        // implement open panel with instructions for the game mechanic of the journal
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
        SceneManager.LoadScene(5);
    }

    void OpenMap()
    {
        mapPanel.SetActive(true);
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

    void CloseInstructions()
    {
        instructionsPanel.SetActive(false);
    }
}
