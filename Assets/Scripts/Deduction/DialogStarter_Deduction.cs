using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using cherrydev;
using Myth_Mystery;


public class DialogStarter_Deduction : MonoBehaviour
{
    #region 
    [Header ("Dialog System")]
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph dialogGraph;

    public int sceneIndex = 1;
    
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogBehaviour.BindExternalFunction("loadNext", loadNextScene);
        dialogBehaviour.StartDialog(dialogGraph);
    }


    void loadNextScene()
    {
        SceneManager.LoadScene(sceneIndex);
    }


}
