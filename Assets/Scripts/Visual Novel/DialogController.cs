using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public static DialogController instance;

    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] TextMeshProUGUI nameText;
    
    [SerializeField] GameObject dialogBox;
    [SerializeField] GameObject answerBox;
    [SerializeField] Button[] answerObjects;

    public static event Action OnDialogStarted;
    public static event Action OnDialogEnded;

    bool skipLineTriggered;
    bool answerTriggered;
    int answerIndex;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void StartDialog(DialogTree dialogTree, int startSection)
    {
        ResetDialog();
        nameText.text = name;
        dialogBox.SetActive(true);
        OnDialogStarted?.Invoke();
        StartCoroutine(RunDialog(dialogTree, startSection));
    }

    IEnumerator RunDialog(DialogTree dialogTree, int section)
    {
        for (int i = 0; i < dialogTree.sections[section].dialog.Length; i++)
        {
            dialogText.text = dialogTree.sections[section].dialog;
            while (skipLineTriggered == false)
            {
                yield return null;
            }
            skipLineTriggered = false;
        }

        if (dialogTree.sections[section].endAfterDialog)
        {
            OnDialogEnded?.Invoke();
            dialogBox.SetActive(false);
            yield break;
        }

        dialogText.text = dialogTree.sections[section].branchPoint.question;
        ShowAnswers(dialogTree.sections[section].branchPoint);

        while(answerTriggered == false)
        {
            yield return null;
        }

        answerBox.SetActive(false);
        answerTriggered = false;

        StartCoroutine(RunDialog(dialogTree, dialogTree.sections[section].branchPoint.answers[answerIndex].nextElement));
    }

    void ResetDialog()
    {
        StopAllCoroutines();
        dialogBox.SetActive(true);
        answerBox.SetActive(true);
        skipLineTriggered = false;
        answerTriggered = false;
    }

    void ShowAnswers(BranchPoint branchPoint)
    {
        answerBox.SetActive(true);
        for(int i = 0; i<3; i++)
        {
            if (i < branchPoint.answers.Length)
            {
                answerObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = branchPoint.answers[i].answerLabel;
                answerObjects[i].gameObject.SetActive(true);
            }
            else
            {
                answerObjects[i].gameObject.SetActive(false);
            }
        }
    }

    public void SkipLine()
    {
        skipLineTriggered=true;
    }

    public void AnswerQuestion(int answer)
    {
        answerIndex = answer;
        answerTriggered = true;
    }

}
