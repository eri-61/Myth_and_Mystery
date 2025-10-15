using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class JournalLoad : MonoBehaviour
{
    public CanvasGroup JournalC;
    public Button closeButton;

    void Start()
    {
        closeButton.gameObject.SetActive(true);
        if (!GameManager.Instance.oldCaseFile)
        {
            JournalC.interactable = false;
            JournalC.blocksRaycasts = false;
            GameManager.Instance.oldCaseFile = true;
        }
        else
        {
            JournalC.interactable = true;
            JournalC.blocksRaycasts = true;
        }
    }

    private void OnEnable()
    {
        closeButton.onClick.AddListener(close);
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(close);
    }

    void close()
    {
        GameManager.Instance.JournalClosed();
    }


}
