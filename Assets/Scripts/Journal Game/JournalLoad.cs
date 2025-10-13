using System.Collections;
using UnityEngine;

public class JournalLoad : MonoBehaviour
{
    public CanvasGroup JournalC;

    void Start()
    {
        if (!GameManager.Instance.oldCaseFile)
        {
            JournalC.interactable = false;
            JournalC.blocksRaycasts = false;

            StartCoroutine(AutoCloseJournal());

            GameManager.Instance.oldCaseFile = true;
        }
        else
        {
            JournalC.interactable = true;
            JournalC.blocksRaycasts = true;
        }
    }

    public IEnumerator AutoCloseJournal()
    {
        yield return new WaitForSeconds(1f);

        int index = GameManager.Instance.GetJournalSceneIndex();
        Debug.Log($"[JournalLoad] Time elapsed — unloading journal scene index {index}.");
        SceneController.Instance.UnloadScene(index);

        GameManager.Instance.JournalClosed();
    }


}
