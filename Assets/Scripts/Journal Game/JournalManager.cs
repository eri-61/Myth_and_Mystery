using UnityEngine;

public class JournalManager : MonoBehaviour
{
    #region Variables
    public static JournalManager instance { get; set; }

    public GameObject journalPanel;

    [Header("Scripts")]
    public CaseFileScript csScript;
    public CluesScript cluesScript;
    public CreaturesScript creaturesScript;
    public SaveLoadScript slScript;
    #endregion

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenJournal()
    {
        journalPanel.SetActive(true);
    }
}

