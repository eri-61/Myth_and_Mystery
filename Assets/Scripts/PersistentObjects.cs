using UnityEngine;

public class PersistentObjects : MonoBehaviour
{
    public static PersistentObjects instance;

    [Header("UI Panels")]
    public GameObject inventoryPanel;
    public GameObject journalPanel;
    public GameObject mapPanel;

    [Header("Camera")]
    public Camera main;

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
        DontDestroyOnLoad(gameObject);

        if (main == null)
        {
            //main = Camera.main;
        }

        if(main == null)
        {
            //GameObject camObj = new GameObject("PersistentMainCamera");
            //main = camObj.AddComponent<Camera>();
            //main.tag = "MainCamera";
        }

        //main.transform.SetParent(transform);
        //DontDestroyOnLoad(main.gameObject);

    }

    public void OpenJournal() => journalPanel.SetActive(true);
    public void CloseJournal() => journalPanel.SetActive(false);    

    public void OpenMap() => mapPanel.SetActive(true);
    public void CloseMap() => mapPanel.SetActive(false);

    public void OpenInventory() => inventoryPanel.SetActive(true);
    public void CloseInventory() => inventoryPanel.SetActive(false);


}
