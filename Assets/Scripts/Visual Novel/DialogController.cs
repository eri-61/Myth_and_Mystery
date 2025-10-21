using System;
using System.Collections;
using System.Threading;
using System.Transactions;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public static DialogController instance;

    [Header("DialogBox")]
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject dialogBox;
    [SerializeField] float typingSpeed;

    [Header("Question and Answer")]
    [SerializeField] GameObject answerBox;
    [SerializeField] Button[] answerObjects;

    [Header("Characters")]
    [SerializeField] GameObject characters;
    [SerializeField] Transform leftAnchor;
    [SerializeField] Transform middleAnchor;
    [SerializeField] Transform rightAnchor;
    public bool isTalking = true;

    [Header("Background")]
    [SerializeField] Image bgImage;

    [Header("Next Scene")]
    [SerializeField] int sceneIndex;
    [SerializeField] Button nextButton;

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

    private void OnEnable()
    {
        nextButton.onClick.AddListener(NextScene);
    }

    private void OnDisable()
    {
        nextButton.onClick.RemoveAllListeners();
    }

    void NextScene()
    {
        SceneController.Instance.LoadScene(sceneIndex);
    }

    private void Update()
    {
        if (!dialogBox.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
            SkipLine();

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            SkipLine();
    }


    public void StartDialog(DialogTree dialogTree, int startSection)
    {
        ResetDialog();
        nextButton.gameObject.SetActive(false);
        dialogBox.SetActive(true);
        answerBox.SetActive(false);
        skipLineTriggered = false;
        OnDialogStarted?.Invoke();
        StartCoroutine(RunDialog(dialogTree, startSection));
    }

    IEnumerator RunDialog(DialogTree dialogTree, int section)
    {
        DialogSection current = dialogTree.sections[section];

        for (int i = 0; i < current.dialog.Length; i++)
        {
            nameText.text = current.characterName[i];
            dialogText.text = "";
            ShowCharacters(current);

            // Set background
            if (current.background != null && current.background.Length > 0)
            {
                Sprite bgToUse = (i < current.background.Length) ? current.background[i] : current.background[current.background.Length - 1];
                if (bgToUse != null && bgImage != null)
                {
                    bgImage.sprite = bgToUse;
                }
            }

            // Find speaker
            Characters? currentSpeaker = null;
            foreach (var c in current.characters)
            {
                if (c.charaName == current.characterName[i])
                {
                    currentSpeaker = c;
                    break;
                }
            }

            // Start talking animation
            if (currentSpeaker.HasValue)
            {
                AudioSource audio = currentSpeaker.Value.characterPrefab.GetComponent<AudioSource>();
                audio.Play();

                Animator animator = currentSpeaker.Value.characterPrefab.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("isTalking", true);

                }
            }

            // Typing effect
            dialogText.text = "";
            skipLineTriggered = false;

            foreach (char letter in current.dialog[i])
            {
                if (skipLineTriggered)
                {
                    dialogText.text = current.dialog[i];
                    break;
                }

                dialogText.text += letter;
                yield return new WaitForSecondsRealtime(typingSpeed);
            }
            
            // End talking
            if (currentSpeaker.HasValue)
            {
                AudioSource audio = currentSpeaker.Value.characterPrefab.GetComponent<AudioSource>();
                audio.Stop();

                Animator anim = currentSpeaker.Value.characterPrefab.GetComponent<Animator>();
                if (anim != null)
                    anim.SetBool("isTalking", false);
            }

            // Wait for click before continuing
            skipLineTriggered = false;
            while (!skipLineTriggered)
            {
                yield return null;
            }
        }

        // Branching logic (after all lines in this section)
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
            {
                StartCoroutine(RunDialog(dialogTree, next));
                yield break;
            }
        }

        // Move to next dialog section
        if (current.nextDialog >= 0 && current.nextDialog < dialogTree.sections.Length)
        {
            StartCoroutine(RunDialog(dialogTree, current.nextDialog));
            yield break;
        }

        // End of dialog
        OnDialogEnded?.Invoke();
        dialogBox.SetActive(false);
        nextButton.gameObject.SetActive(true);
    }

    void ShowCharacters(DialogSection section)
    {
        foreach (var c in section.characters)
        {
            if (c.characterPrefab == null) continue;

            Transform parentAnchor = characters.transform;

            switch (c.position)
            {
                case CharacterPosition.Left:
                    parentAnchor = leftAnchor;
                    break;
                case CharacterPosition.Middle:
                    parentAnchor = middleAnchor;
                    break;
                case CharacterPosition.Right:
                    parentAnchor = rightAnchor;
                    break;
            }

            GameObject newChar = Instantiate(c.characterPrefab, parentAnchor);
            newChar.transform.localScale = Vector3.one;
            newChar.transform.localPosition = Vector3.zero;
            newChar.transform.localRotation = Quaternion.identity;
        }
    }

    void ResetDialog()
    {
        dialogBox.SetActive(true);
        answerBox.SetActive(false);
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
