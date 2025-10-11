using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class JournalManager : MonoBehaviour
{
    #region Journal Variables
    [Header("Journal Buttons")]
    public Button caseButton;
    public Button cluesBtn;
    public Button slBtn;
    public Button creaturesBtn;
   
    [Header("Close Button")]
    public Button closeBtn;
    #endregion

    #region Case File Variables
    [Header("Case File UI")]
    public Image caseImage;
    public TMProUGUI caseObjectivesText;

    [Header("Data")]
    public CaseFileData currentCaseFile;
    #endregion

    #region Clues Variables
    [Header("Data")]
    public List<CluesData> collectedClues = new();
    public List<TestimonyData> collectedTestimonies = new();
    #endregion

    #region Creatures Variables

    #endregion

    void OnEnable()
    {
        caseButton.onClick.AddListener(OpenCaseFile);
        cluesBtn.onClick.AddListener(openClues);
        slBtn.onClick.AddListener(openSL);
        creaturesBtn.onClick.AddListener(openCreatures);
        closeBtn.onClick.AddListener(CloseCaseFile);
    }

    void OnDisable()
    {
        caseButton.onClick.RemoveListener(OpenCaseFile);
        cluesBtn.onClick.RemoveListener(openClues);
        slBtn.onClick.RemoveListener(openSL);
        creaturesBtn.onClick.RemoveListener(openCreatures);
        closeBtn.onClick.RemoveListener(CloseCaseFile);
    }

    //journal - clues
    public void AddClue(CluesData clue)
    {
        if (!collectedClues.Contains(clue))
        {
            collectedClues.Add(clue);
        }
        UpdateCluesUI();
    }

    public void AddTestimony(TestimonyData testimony)
    {
        if (!collectedTestimonies.Contains(testimony))
        {
            collectedTestimonies.Add(testimony);
        }
        UpdateCluesUI();
    }

    //journal - case file
    public void CompleteObjective()
    {
        if (currentCaseFile != null && index >= 0 && index < currentCaseFile.objectives.Length)
        {
            currentCaseFile.objectives[index].isCompleted = true;
            UpdateCaseFileUI();
        }
    }

    public void UpdateCaseFileUI()
    {
        if (currentCaseFile == null) return;
        if (caseImage != null)
            caseImage.sprite = currentCaseFile.caseImage;

        if (currentCaseFile.objectives != null && currentCaseFile.objectives.Length > 0)
        {
            string formattedObjectives = "";

            foreach (var obj in currentCaseFile.objectives)
            {
                if (obj.isCompleted)
                    formattedObjectives += $"<s>{obj.description}</s>\n"; // strikethrough
                else
                    formattedObjectives += $"{obj.description}\n";
            }

            caseObjectivesText.text = formattedObjectives;
        }
        else
        {
            caseObjectivesText.text = "No objectives available.";
        }
    }

    public void UpdateCluesUI()
    {
        // add clues UI update logic here
    }

    public void CloseCaseFile()
    {
        if (PlayerPrefs.HasKey("PreviousScene"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("PreviousScene"));
        }
    }
    //open tabs
    void OpenCaseFile()
    {
        UpdateCaseFileUI();
    }

    void openClues()
    {
        UpdateCluesUI();
    }

    void openSL()
    {
        // UpdateSLUI();
    }

    void openCreatures()
    {
        // UpdateCreaturesUI();
    }
}

