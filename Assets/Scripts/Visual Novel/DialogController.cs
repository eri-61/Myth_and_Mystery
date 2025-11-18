using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  

public class DialogController : MonoBehaviour
{
    public static DialogController instance;

    [Header("DialogBox")]
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] GameObject dialogBox;
    [SerializeField] float typingSpeed = 0.05f;

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
    [SerializeField] public int sceneIndex;
    [SerializeField] Button nextButton;
    [SerializeField] private bool needsItem = false;
    [SerializeField] private string requiredItemName;

    [Header("Warning")]
    [SerializeField] private string speakerName = "Player";
    [SerializeField] [TextArea]
    private string warningMessage = "You need a specific item to proceed.";

    // Current section the dialog is running (set while RunDialog is active)
    public int currentSectionIndex { get; private set; } = -1;

    public static event Action OnDialogStarted;
    public static event Action OnDialogEnded;

    bool skipLineTriggered;
    bool answerTriggered;
    int answerIndex;

    bool skipAll = false;
    float savedTypingSpeed;

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
        if (needsItem)
        {
            string selectedItemName = InventoryManager.instance.GetSelectedName();
            bool itemIsCorrect = selectedItemName != null && selectedItemName.Equals(requiredItemName, System.StringComparison.OrdinalIgnoreCase);
            
            if (itemIsCorrect)
            {
                Items usedItem = InventoryManager.instance.GetSelectedItem(true);

                if (usedItem != null)
                {
                    Debug.Log($"Used item: {usedItem.name}. Proceeding to scene {sceneIndex}.");
                    SceneManager.LoadScene(sceneIndex);
                    return;
                }

                StartCoroutine(ShowItemRequiredWarning());
            }
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
            return;
        }
       
    }

    IEnumerator ShowItemRequiredWarning()
    {
        dialogBox.SetActive(true);
        answerBox.SetActive(false);
        nextButton.gameObject.SetActive(false);

        nameText.text = speakerName;

        string finalMessage = warningMessage.Replace("[requiredItemName]", requiredItemName);

        dialogText.text = "";
        skipLineTriggered = false;

        // Type the warning text
        foreach (char letter in finalMessage)
        {
            if (skipLineTriggered)
            {
                dialogText.text = finalMessage;
                break;
            }

            dialogText.text += letter;

            if (!skipAll)
                yield return new WaitForSecondsRealtime(typingSpeed);
        }

        skipLineTriggered = false;
        while (!skipLineTriggered)
            yield return null;

        dialogText.text = "";
        nameText.text = "";

        Debug.Log("Item requirement warning dismissed.");
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

        // ensure currentSectionIndex reflects the starting section
        currentSectionIndex = startSection;

        StartCoroutine(RunDialog(dialogTree, startSection));
    }

    IEnumerator RunDialog(DialogTree dialogTree, int section)
    {
        // update current section index so external handlers can inspect it
        currentSectionIndex = section;

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

            // Use GetComponentInChildren so we find Animator/AudioSource placed on child Image/GameObject
            if (activeCharacterInstance != null)
            {
                var audio = activeCharacterInstance.GetComponentInChildren<AudioSource>();
                if (audio != null)
                    audio.Play();

                var animator = activeCharacterInstance.GetComponentInChildren<Animator>();
                if (animator != null)
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

                if (skipAll)
                    yield return null;
                else
                    yield return new WaitForSecondsRealtime(typingSpeed);

            }
            
            if (activeCharacterInstance != null)
            {
                var audio = activeCharacterInstance.GetComponentInChildren<AudioSource>();
                if (audio != null)
                    audio.Stop();

                var anim = activeCharacterInstance.GetComponentInChildren<Animator>();
                if (anim != null)
                    anim.SetBool("isTalking", false);
            }

            // Wait for click before continuing
            skipLineTriggered = false;

            if (skipAll)
            {
                yield return null;
            }
            else
            {
                while (!skipLineTriggered)
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
        // Clear previously spawned instances under the parent
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

        // Primary behavior: use the prefab at the same index in section.characters (parallel arrays).
        GameObject prefabToSpawn = null;

        if (section.characters != null && index < section.characters.Length)
        {
            var candidate = section.characters[index];
            if (candidate.characterPrefab != null)
            {
                prefabToSpawn = candidate.characterPrefab;

                if (!string.IsNullOrEmpty(candidate.charaName) && candidate.charaName != speakerName)
                    Debug.LogWarning($"DialogSection.characters[{index}].charaName ('{candidate.charaName}') does not match speaker name ('{speakerName}'). Using prefab by index anyway.");
            }
        }

        // Fallback: find first prefab that matches speakerName (name-based setup)
        if (prefabToSpawn == null && section.characters != null)
        {
            foreach (var c in section.characters)
            {
                if (c.characterPrefab == null) continue;
                if (c.charaName != speakerName) continue;
                prefabToSpawn = c.characterPrefab;
                break;
            }
        }

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"No character prefab found for '{speakerName}'.");
            return;
        }

        if (charactersParent != null)
            activeCharacterInstance = Instantiate(prefabToSpawn, charactersParent.transform, false);
        else
            activeCharacterInstance = Instantiate(prefabToSpawn);

        SetVFXVolumeOnInstance(activeCharacterInstance, AudioManager.Instance != null ? AudioManager.Instance.VFXVolume : 1f);
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


    void SetVFXVolumeOnInstance(GameObject inst, float vol)
    {
        if (inst == null) return;
        var audios = inst.GetComponentsInChildren<AudioSource>();
        foreach (var a in audios)
            a.volume = vol;
    }

    void OnVFXVolumeChanged(float vol)
    {
        SetVFXVolumeOnInstance(activeCharacterInstance, vol);
    }

    public void SkipLine()
    {
        skipLineTriggered = true;
    }

    public void ToggleSkipAll(bool on)
    {
        if (on == skipAll) return;
        skipAll = on;
        if (skipAll)
        {
            savedTypingSpeed = typingSpeed;
            typingSpeed = 0f;
        }
        else
        {
            typingSpeed = savedTypingSpeed > 0f ? savedTypingSpeed : 0.04f;
        }
    }

    public void SetTypingSpeed(float speed)
    {
        typingSpeed = Mathf.Max(0f, speed);
    }

    public void AnswerQuestion(int answer)
    {
        answerIndex = answer;
        answerTriggered = true;
    }

}
