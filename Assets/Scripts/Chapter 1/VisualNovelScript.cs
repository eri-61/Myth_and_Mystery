using UnityEngine;
using UnityEngine.UI;

public class VisualNovelScript : MonoBehaviour
{
    [Header("Visual Novel Buttons")]
    public Button autoBtn;
    public Button speedBtn;
    public Button hideBtn;
    public Button logBtn;

    [Header("Other Buttons")]
    public Button inventoryBtn;
    public Button menuBtn;
    public Button journalBtn;
    public Button mapBtn;

    [Header("Panels")]
    public GameObject inventoryPanel;
    public GameObject menuPanel;
    public GameObject journalPanel;
    public GameObject mapPanel;
    public GameObject logPanel;
    public GameObject uiPanel;

    [Header("Close All Panels Button")]
    public Button closeAllBtn;

    void onEnable()
    {
        autoBtn.onClick.AddListener(AutoPlay);
        speedBtn.onClick.AddListener(ChangeSpeed);
        hideBtn.onClick.AddListener(HideUI);
        logBtn.onClick.AddListener(OpenLog);
        inventoryBtn.onClick.AddListener(OpenInventory);
        menuBtn.onClick.AddListener(OpenMenu);
        journalBtn.onClick.AddListener(OpenJournal);
        mapBtn.onClick.AddListener(OpenMap);
        closeAllBtn.onClick.AddListener(CloseAllPanels);
    }

    void onDisable()
    {
        autoBtn.onClick.RemoveListener(AutoPlay);
        speedBtn.onClick.RemoveListener(ChangeSpeed);
        hideBtn.onClick.RemoveListener(HideUI);
        logBtn.onClick.RemoveListener(OpenLog);
        inventoryBtn.onClick.RemoveListener(OpenInventory);
        menuBtn.onClick.RemoveListener(OpenMenu);
        journalBtn.onClick.RemoveListener(OpenJournal);
        mapBtn.onClick.RemoveListener(OpenMap);
        closeAllBtn.onClick.RemoveListener(CloseAllPanels);
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

    void OpenLog()
    {
        // Implement open log functionality
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
        journalPanel.SetActive(true);
    }

    void OpenMap()
    {
        mapPanel.SetActive(true);
    }

    void CloseAllPanels()
    {
        inventoryPanel.SetActive(false);
        menuPanel.SetActive(false);
        journalPanel.SetActive(false);
        mapPanel.SetActive(false);
        logPanel.SetActive(false);
    }
}
