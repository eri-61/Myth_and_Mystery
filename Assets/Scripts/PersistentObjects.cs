using UnityEngine;

public class PersistentObjects : MonoBehaviour
{
    public static PersistentObjects instance;

    [Header("UI Panels")]
    public GameObject inventoryPanel;
    public GameObject journalPanel;
    public GameObject menuPanel;
    public GameObject mapPanel;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        mapPanel.SetActive(false);
        journalPanel.SetActive(false);
        inventoryPanel.SetActive(false);
        menuPanel.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void OpenJournal() => journalPanel.SetActive(true);
    public void CloseJournal() => journalPanel.SetActive(false);    

    public void OpenMap() => mapPanel.SetActive(true);
    public void CloseMap() => mapPanel.SetActive(false);

    public void OpenInventory() => inventoryPanel.SetActive(true);
    public void CloseInventory() => inventoryPanel.SetActive(false);

    public void OpenSettings() => menuPanel.SetActive(true);
    public void CloseSettings() => menuPanel.SetActive(false);

}
