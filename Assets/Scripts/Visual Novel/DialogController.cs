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
    [SerializeField] GameObject charactersParent; 
    GameObject activeCharacterInstance; 
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
            ShowCharacters(current, i);

            if (current.background != null && current.background.Length > 0)
            {
                Sprite bgToUse = (i < current.background.Length) ? current.background[i] : current.background[current.background.Length - 1];
                if (bgToUse != null && bgImage != null)
                {
                    bgImage.sprite = bgToUse;
                }
            }

            if (activeCharacterInstance != null)
            {
                if (activeCharacterInstance.TryGetComponent(out AudioSource audio))
                {
                    audio.Play();
                }

                if (activeCharacterInstance.TryGetComponent(out Animator animator))
                {
                    animator.SetBool("isTalking", true);
                }
            }

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
            
            if (activeCharacterInstance != null)
            {
                if (activeCharacterInstance.TryGetComponent(out AudioSource audio))
                {
                    audio.Stop();
                }

                if (activeCharacterInstance.TryGetComponent(out Animator anim))
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
            // If there are no answers, skip branching
            if (current.branchPoint.answers == null || current.branchPoint.answers.Length == 0)
            {
                Debug.LogWarning("BranchPoint has a question but no answers defined. Skipping branch.");
            }
            else
            {
                dialogText.text = current.branchPoint.question;
                ShowAnswers(current.branchPoint);

                while (!answerTriggered)
                    yield return null;

                // Safely clamp answerIndex
                int chosen = Mathf.Clamp(answerIndex, 0, current.branchPoint.answers.Length - 1);

                // Remove listeners to avoid duplicate handlers next time
                for (int i = 0; i < answerObjects.Length; i++)
                    answerObjects[i].onClick.RemoveAllListeners();

                answerBox.SetActive(false);
                answerTriggered = false;

                int next = current.branchPoint.answers[chosen].nextElement;
                if (next >= 0 && next < dialogTree.sections.Length)
                {
                    StartCoroutine(RunDialog(dialogTree, next));
                    yield break;
                }
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

    void ShowCharacters(DialogSection section, int index)
    {
        if (charactersParent != null)
        {
            for (int i = charactersParent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(charactersParent.transform.GetChild(i).gameObject);
            }
        }

        activeCharacterInstance = null;

        string speakerName = (section.characterName != null && index < section.characterName.Length) ? section.characterName[index] : null;
        if (string.IsNullOrEmpty(speakerName)) return;

        foreach (var c in section.characters)
        {
            if (c.characterPrefab == null) continue;
            if (c.charaName != speakerName) continue;

            Transform parentAnchor = (charactersParent != null) ? charactersParent.transform : null;

            activeCharacterInstance = Instantiate(c.characterPrefab, parentAnchor);
            activeCharacterInstance.transform.localScale = c.characterPrefab.transform.localScale;

            activeCharacterInstance.transform.localPosition = Vector3.zero;
            activeCharacterInstance.transform.localRotation = Quaternion.identity;
            break;
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
        if (branchPoint.answers == null || branchPoint.answers.Length == 0)
        {
            Debug.LogWarning("ShowAnswers called with no answers. Hiding answer box.");
            answerBox.SetActive(false);
            return;
        }

        answerBox.SetActive(true);

        int btnCount = answerObjects != null ? answerObjects.Length : 0;
        for (int i = 0; i < btnCount; i++)
        {
            var btn = answerObjects[i];
            btn.onClick.RemoveAllListeners();

            if (i < branchPoint.answers.Length)
            {
                int captured = i; 
                btn.onClick.AddListener(() => AnswerQuestion(captured));

                var label = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null)
                    label.text = branchPoint.answers[i].answerLabel;
                else
                    Debug.LogWarning($"Answer button at index {i} has no TextMeshProUGUI child.");

                btn.gameObject.SetActive(true);
            }
            else
            {
                btn.gameObject.SetActive(false);
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
