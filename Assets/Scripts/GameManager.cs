using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region
    public static GameManager Instance;
    public bool isNewGame = false;
    public int newGameScene = 1;

    [Header("Journal")]
    public CaseFileScript cfScript;
    public CluesScript cluesScript;
    public CreaturesScript creaturesScript;
    public SaveLoadScript slScript;
    #endregion
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame()
    {
        isNewGame = true;
        SceneManager.LoadScene(newGameScene);
    }

    public void LoadChapter(int index)
    {
        isNewGame = false;
        SceneManager.LoadScene(index);
    }

    public void LoadSave()
    {
        isNewGame = false;
        //add code
    }

    public void RegisterCaseFile(CaseFileScript caseFile)
    {
        cfScript = caseFile;
    }

    public void RegisterClues(CluesScript clues)
    {
        cluesScript = clues;
    }

    public void RegisterCreatures(CreaturesScript creatures)
    {
        creaturesScript = creatures;
    }

    public void RegisterSaveLoad(SaveLoadScript sl)
    {
        slScript = sl;
    }
}
