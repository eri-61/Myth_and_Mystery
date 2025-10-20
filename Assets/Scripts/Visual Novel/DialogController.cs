using System;
using System.Collections;
using System.Transactions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public static DialogController instance;

    [Header ("DialogBox")]
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject dialogBox;
    [SerializeField] float typingSpeed;

    [Header ("Question and Answer")]
    [SerializeField] GameObject answerBox;
    [SerializeField] Button[] answerObjects;

    [Header ("Characters")]
    [SerializeField] GameObject characters;
    public bool isTalking = true;

    [Header("Background")]
    Image bgImage;

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
        dialogBox.SetActive(true);
        answerBox.SetActive(false);
        OnDialogStarted?.Invoke();
        StartCoroutine(RunDialog(dialogTree, startSection));
    }

    IEnumerator RunDialog(DialogTree dialogTree, int section)
    {
        DialogSection current = dialogTree.sections[section];

        ShowCharacters(current);

        for (int i = 0; i < current.dialog.Length; i++)
        {
            nameText.text = current.characterName[i];
            dialogText.text = "";

            if (current.background != null && current.background.Length > 0)
            {
                Sprite bgToUse = (i < current.background.Length) ? current.background[i] : current.background[current.background.Length - 1];
                if (bgToUse != null && bgImage != null)
                {
                    bgImage.sprite = bgToUse;
                }
            }

            Characters? currentSpeaker = null;
            foreach (var c in current.characters)
            {
                if (c.charaName == current.characterName[i])
                {
                    currentSpeaker = c;
                    break;
                }
            }

            if (currentSpeaker.HasValue)
            {
                Animator animator = currentSpeaker.Value.characterPrefab.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("isTalking", true);
                }
            }

            foreach (char letter in current.dialog[i])
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }

            if (currentSpeaker.HasValue)
            {
                Animator anim = currentSpeaker.Value.characterPrefab.GetComponent<Animator>();
                if (anim != null)
                {
                    anim.SetBool("isTalking", false);
                }
            }

            skipLineTriggered = false;
            while (skipLineTriggered)
            {
                yield return null;
            }
        }

        if (!string.IsNullOrEmpty(current.branchPoint.question))
        {
            dialogText.text = current.branchPoint.question;
            ShowAnswers(current.branchPoint);

            while (!answerTriggered)
                yield return null;

            answerBox.SetActive(false);
            answerTriggered = false;

            int next = current.branchPoint.answers[answerIndex].nextElement;
            if (next >= 0 && next < dialogTree.sections.Length)
                StartCoroutine(RunDialog(dialogTree, next));

            yield break;
        }

        if (current.nextDialog >= 0 && current.nextDialog < dialogTree.sections.Length)
        {
            StartCoroutine(RunDialog(dialogTree, current.nextDialog));
            yield break;
        }

        OnDialogEnded?.Invoke();
        dialogBox.SetActive(false);
    }

    void ShowCharacters(DialogSection section)
    {
        foreach (Transform child in characters.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var c in section.characters)
        {
            if (c.characterPrefab == null || c.transform == null) continue;

            GameObject newChar = Instantiate(c.characterPrefab, c.transform);
            newChar.transform.localScale = Vector3.one;
        }

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
