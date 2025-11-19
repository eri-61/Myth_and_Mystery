using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    #endregion

    public enum Dialog { RIGHT, WRONG }
    private Dialog lastDeductionResult;
    private enum EntryType { Clue, Testimony }
    private List<EntryType> entrytypes = new();

    void Awake()
    {
        if (deductionButton != null)
        {
            deductionButton.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            int index = i;
            slots[i].onClick.RemoveAllListeners();
            slots[i].onClick.AddListener(() => showDetails(index));
        }

        deductionButton.onClick.RemoveAllListeners();
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
        if (index < 0 || index >= gatheredClues.Count) return;

        var entry = gatheredClues[index];
        string prefix = (entry.entryType == JournalEntryType.Clue) ? "Clue: " : "Testimony: ";

        entryName.text = prefix + entry.clueName;
        entryDescription.text = entry.clueDescription;
        entryImage.sprite = entry.clueImage;
    }

    public void goToDeductionMode()
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void DeductionComplete(Dialog result)
    {
        lastDeductionResult = result;

        if (deductionButton != null)
        {
            if (result == Dialog.RIGHT)
            {
                deductionButton.gameObject.SetActive(true);
                deductionButton.interactable = false;
                entryDescription.text = "<b>Deduction Completed.</b>";
                Debug.Log("[CluesScript] Deduction Completed Successfully.");
            }
            else if (result == Dialog.WRONG)
            {
                deductionButton.gameObject.SetActive(true);
                deductionButton.interactable = false;
                entryDescription.text = "<b>Wrong Deduction. Try again.</b>";
                Debug.Log("[CluesScript] Deduction was incorrect.");
            }
        }
        else
        {
            Debug.Log("[CluesScript] Deduction result stored; UI not present to update now.");
        }
    }

    public void AddClues(CluesData newEntry)
    {
        if (newEntry == null) return;

        if (!gatheredClues.Contains(newEntry))
        {
            gatheredClues.Add(newEntry);

            UpdateClueUI();
            CheckDeductionButton();
        }
    }

    public void UpdateClueUI()
    {
        int totalEntries = gatheredClues.Count;

        for (int i = 0; i < slots.Count; i++)
        {
            Image slotImage = slots[i].GetComponent<Image>();

            if (i < totalEntries)
            {
                slotImage.sprite = gatheredClues[i].clueIcon;
                slots[i].interactable = true;
            }
            else
            {
                // Clear the slot if there's no entry
                slotImage.sprite = null;
                slots[i].interactable = false;
            }
        }
        CheckDeductionButton();
    }

    private void CheckDeductionButton()
    {
        int totalGathered = gatheredClues.Count;
        
        if (allCluesAndTestimonies > 0 && totalGathered >= allCluesAndTestimonies)
        {
            deductionButton.gameObject.SetActive(true);
        }
        else
        {
            deductionButton.gameObject.SetActive(false);
        }
    }
}
