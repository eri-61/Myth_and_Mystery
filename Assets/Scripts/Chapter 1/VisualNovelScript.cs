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

    [Header("Close  Panels Button")]
    public Button closeInv;
    public Button closeMap;
    public Button closeMenu;
    public Button closeJournal;
    public Button closeLog;

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
        closeInv.onClick.AddListener(ClosePanels);
        closeMap.onClick.AddListener(ClosePanels);
        closeMenu.onClick.AddListener(ClosePanels);
        closeJournal.onClick.AddListener(ClosePanels);
        closeLog.onClick.AddListener(ClosePanels);
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
        closeInv.onClick.RemoveListener(ClosePanels);
        closeMap.onClick.RemoveListener(ClosePanels);
        closeMenu.onClick.RemoveListener(ClosePanels);
        closeJournal.onClick.RemoveListener(ClosePanels);
        closeLog.onClick.RemoveListener(ClosePanels);
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

    void ClosePanels()
    {
        if (inventoryPanel.activeSelf) inventoryPanel.SetActive(false);
        if (menuPanel.activeSelf) menuPanel.SetActive(false);
        if (journalPanel.activeSelf) journalPanel.SetActive(false);
        if (mapPanel.activeSelf) mapPanel.SetActive(false);
        if (logPanel.activeSelf) logPanel.SetActive(false);
    }
}
