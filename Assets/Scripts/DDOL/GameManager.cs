using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{

    #region Variables
    public static GameManager Instance;
    public CaseFileScript cfScript;
    public CluesScript cluesScript;
    public CreaturesScript creaturesScript;

    public bool playOnStart = true;
    #endregion

    private void Awake()
    {
        if (Instance != null)
        {
            //CleanUpAndDestroy();
            //return;
        }
        else
        {
            //Instance = this;
            //DontDestroyOnLoad(gameObject);
            //ontDestroyOnLoad(persistentObjects.gameObject);
        }
    }

    private void CleanUpAndDestroy()
    {
        Destroy(persistentObjects.gameObject);
        Destroy(gameObject);
    }

    public void RegitsterCaseFile(CaseFileScript file)
    {
        cfScript = file;
    }

    public void RegisterClues(CluesScript clues)
    {
        cluesScript = clues;
    } 

    public void RegisterCreatures(CreaturesScript creatures)
    {
        creaturesScript = creatures;
    }
}