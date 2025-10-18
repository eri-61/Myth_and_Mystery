using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CluesScript : MonoBehaviour
{
    #region Variables
    [Header("Slots")]
    public List<Button> slots;

    [Header("Deduction Mode")]
    public Button deductionButton;

    [Header("Clues Details")]
    public TextMeshProUGUI entryName;
    public Image entryImage;
    public TextMeshProUGUI entryDescription;

    [Header("Variables and Data")]
    public int sceneIndex = 1;
    public int allCluesAndTestimonies = 0; 
    public List<CluesData> gatheredClues = new();
    public List<TestimonyData> gatheredTestimonies = new();
    #endregion

    public Dialog result;
    private enum EntryType { Clue, Testimony }
    private List<EntryType> entrytypes = new();

    void Awake()
    {
        deductionButton.gameObject.SetActive(false);
        GameManager.Instance.RegisterClues(this);

    }

    void OnEnable()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            int index = i;
            slots[i].onClick.AddListener(() => showDetails(index));
        }

        deductionButton.onClick.AddListener(goToDeductionMode);
        UpdateClueUI();
    }

    void OnDisable()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].onClick.RemoveAllListeners();
        }

        deductionButton.onClick.RemoveListener(goToDeductionMode);
    }

    public void showDetails(int index)
    {
        if (entrytypes[index] == EntryType.Clue)
        {
            var clue = gatheredClues[index];
            entryName.text = "Clue: " + clue.clueName;
            entryDescription.text = clue.clueDescription;
            entryImage.sprite = clue.clueImage;
        }
        else if (entrytypes[index] == EntryType.Testimony)
        {
            var testimony = gatheredTestimonies[index - gatheredClues.Count];
            entryName.text = "Witness: " + testimony.witnessName;
            entryDescription.text = testimony.testimonyText;
            entryImage.sprite = testimony.witnessPortrait;
        }
    }

    public void goToDeductionMode()
    {
        SceneController.Instance.LoadScene(sceneIndex);
    }

    public void DeductionComplete(Dialog result)
    {
        this.result = result;

        if (deductionButton != null)
        {
            if (result == Dialog.RIGHT)
            {
                deductionButton.interactable = false;
                entryDescription.text = "<b> Deduction Completed.</b>";
            }
            else if (result == Dialog.WRONG)
            {
                entryDescription.text = "<b>Wrong Deduction.</b>";
            }
        }
        else
        {
            Debug.Log("[CluesScript] Deduction result stored; UI not present to update now.");

        }
    }

    public void AddClues(CluesData newClue)
    {
        if (!gatheredClues.Contains(newClue))
        {
            gatheredClues.Add(newClue);
            entrytypes.Add(EntryType.Clue);

            UpdateClueUI();
            CheckDeductionButton();
        }
    }

    public void addTestimony(TestimonyData newTestimony)
    {
        if (!gatheredTestimonies.Contains(newTestimony))
        {
            gatheredTestimonies.Add(newTestimony);
            entrytypes.Add(EntryType.Testimony);

            UpdateClueUI();
            CheckDeductionButton();
        }
    }

    private void UpdateClueUI()
    {
        int totalEntries = gatheredClues.Count + gatheredTestimonies.Count;

        for (int i = 0; i < slots.Count; i++)
        {
            Image slotImage = slots[i].GetComponent<Image>();

            if (i < totalEntries)
            {
                if (i < gatheredClues.Count)
                {
                    slotImage.sprite = gatheredClues[i].clueIcon;
                }
                else
                {
                    int tIndex = i - gatheredClues.Count;
                    slotImage.sprite = gatheredTestimonies[tIndex].testimonyIcon;
                }
                slots[i].interactable = true;
            }
            else
            {
                slots[i].interactable = false;
            }
        }
        CheckDeductionButton();
    }

    private void CheckDeductionButton()
    {
        int totalGathered = gatheredClues.Count + gatheredTestimonies.Count;
        if (totalGathered >= allCluesAndTestimonies)
        {
            deductionButton.gameObject.SetActive(true);
        }
        else
        {
            deductionButton.gameObject.SetActive(false);
        }
    }
}
