using System.Collections;
using UnityEngine;

public class JournalLoad : MonoBehaviour
{
    public CanvasGroup JournalC;

    void Start()
    {
        if (GameManager.Instance.oldCaseFile = false)
        {
            JournalC.interactable = false;
            JournalC.blocksRaycasts = false;
            StartCoroutine(LoadJournal());
            GameManager.Instance.oldCaseFile = true;
        }
        else
        {
            JournalC.interactable = true;
            JournalC.blocksRaycasts = true;
        }
    }

    public IEnumerator LoadJournal()
    {
        yield return new WaitForSeconds(10f);
        int index = GameManager.Instance.GetJournalSceneIndex();
        SceneController.Instance.UnloadScene(index);
    }


}
