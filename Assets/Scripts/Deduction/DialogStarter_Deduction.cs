using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using cherrydev;
using Myth_Mystery;

public enum Dialog
{
    NONE,
    WRONG,
    RIGHT
}
public class DialogStarter_Deduction : MonoBehaviour
{
    
    #region 
    [Header ("Dialog System")]
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph dialogGraph;

    [Header ("scene index")]
    public int sceneIndex = 1;
    #endregion

    public Dialog result = Dialog.NONE;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogBehaviour.BindExternalFunction("loadNext", loadNextScene);
        dialogBehaviour.BindExternalFunction("goBack", GoBackToPrevious);
        dialogBehaviour.StartDialog(dialogGraph);
    }


    void loadNextScene()
    {
        result = Dialog.RIGHT;
        SceneController.Instance.LoadScene(sceneIndex);
    }

    void GoBackToPrevious()
    {
        result = Dialog.WRONG;
        SceneController.Instance.LoadScene(sceneIndex);
    }


}
