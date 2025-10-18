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

    [Header("Data")]
    public TestimonyData testimony;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogBehaviour.BindExternalFunction("loadNext", loadNextScene);
        dialogBehaviour.BindExternalFunction("goBack", GoBackToPrevious);
        dialogBehaviour.BindExternalFunction("addTestimony", addTestimony);

        dialogBehaviour.StartDialog(dialogGraph);
    }

    public void addTestimony() => GameManager.Instance.cluesScript.addTestimony(testimony);

    void loadNextScene()
    {
        if (GameManager.Instance != null && GameManager.Instance.cluesScript)
        {
            GameManager.Instance.cluesScript.DeductionComplete(Dialog.RIGHT);
        }
        else
        {
            var clue = FindAnyObjectByType<CluesScript>();
            if (clue != null)
            {
                clue.DeductionComplete(Dialog.RIGHT);
            }
        }
        SceneController.Instance.LoadScene(sceneIndex);
    }

    void GoBackToPrevious()
    {
        if (GameManager.Instance != null && GameManager.Instance.cluesScript)
        {
            GameManager.Instance.cluesScript.DeductionComplete(Dialog.WRONG);
        }
        else
        {
            var clue = FindAnyObjectByType<CluesScript>();
            if (clue != null)
            {
                clue.DeductionComplete(Dialog.WRONG);
            }
        }
        SceneController.Instance.LoadScene(sceneIndex);
    }


}
