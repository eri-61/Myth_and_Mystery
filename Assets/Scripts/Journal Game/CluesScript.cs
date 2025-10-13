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
    public List<CluesData> gatheredClues = new();
    public List<TestimonyData> gatheredTestimonies = new();
    #endregion

    private enum EntryType { Clue, Testimony }
    private List<EntryType> entrytypes = new();
    
    void Awake()
    {
        if (GameManager.Instance != null) 
        {
            GameManager.Instance.RegisterClues(this);
        }
    }

    void OnEnable()
    {
        for (int i = 0; i < slots.Count; i++) 
        {
            int index = i;
            slots[i].onClick.AddListener(() =>showDetails(index));
        }

        deductionButton.onClick.AddListener(goToDeductionMode);
        UpdateClueUI();
    }

    void OnDisable()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            int index = i;
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

    public void deductionComplete()
    {
        deductionButton.interactable = false;
        entryDescription.text += "\n\n<b>Deduction Complete!</b>";
    }

    public void AddClues(CluesData newClue)
    {
        if (!gatheredClues.Contains(newClue))
        {
            gatheredClues.Add(newClue);
            entrytypes.Add(EntryType.Clue);

            UpdateClueUI();
        }
    }

    public void addTestimony(TestimonyData newTestimony)
    {
        if (!gatheredTestimonies.Contains(newTestimony))
        {
            gatheredTestimonies.Add(newTestimony);
            entrytypes.Add(EntryType.Testimony);

            UpdateClueUI();
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
                if ( i< gatheredClues.Count)
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
                slotImage.sprite = null;
                slots[i].interactable = false;
            }
        }
    }
}
