using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Text;

public class CaseFileScript : MonoBehaviour
{
    #region Variables
    [Header("Case File UI")]
    public Image caseImage;
    public TextMeshProUGUI objectiveText;

    [Header("Data")]
    public CaseFileData currentCaseFile;
    public ObjectiveData objectves;
    #endregion
    void Awake() 
    {
        GameManager.Instance?.RegisterCaseFile(this);
    }

    private void Start()
    {
        UpdateCaseFileUI();    
    }

    public void CompleteObjective(int index)
    {
        if (currentCaseFile == null || currentCaseFile.objectives == null) { return;}

        if (currentCaseFile != null && index >= 0 && index < currentCaseFile.objectives.Length)
        {
            currentCaseFile.objectives[index].isCompleted = true;
            UpdateCaseFileUI();
        }
    }

    public void RevealObjective(int index)
    {
        if (currentCaseFile == null || currentCaseFile.objectives == null) return;
        if (index < 0 || index >= currentCaseFile.objectives.Length) return;

        currentCaseFile.objectives[index].isVisible = true;
        UpdateCaseFileUI();
    }


    public void UpdateCaseFileUI()
    {
        if (currentCaseFile == null)
        {
            objectiveText.text = "There is no ongoing case.";
        }

        if (caseImage != null)
            caseImage.sprite = currentCaseFile.caseImage;

        StringBuilder formattedObjective = new StringBuilder();

        if (currentCaseFile.objectives != null && currentCaseFile.objectives.Length > 0)
        {
            var mainObjectives = currentCaseFile.objectives
                                 .Where(o => o.isMainObjective && o.isVisible)
                                 .ToList();

            var sideObjective = currentCaseFile.objectives
                                .Where(o => !o.isMainObjective && o.isVisible)
                                .ToList();

            //main
            if (mainObjectives.Count> 0)
            {
                formattedObjective.AppendLine("<b>Main Objectives:</b>");
                for (int i = 0; i < mainObjectives.Count; i++)
                {
                    var obj = mainObjectives[i];
                    string displayText = obj.isCompleted
                        ? $"<s>{i + 1}.{obj.description}</s>"
                        : $"{i+1}.{obj.description}";

                    formattedObjective.AppendLine(displayText);
                }
                formattedObjective.AppendLine();
            }

            //side
            if (sideObjective.Count> 0)
            {
                formattedObjective.AppendLine("<b>Side Objectives:</b>");
                for (int i = 0;i < sideObjective.Count; i++)
                {
                    var obj = sideObjective[i];
                    string displayText = obj.isCompleted
                        ? $"<s>{i + 1}.{obj.description}</s>"
                        : $"{i + 1}.{obj.description}";

                    formattedObjective.AppendLine(displayText);
                }
            }

            if (formattedObjective.Length == 0) 
            {
                formattedObjective.Append("No objectives available.");
            }

            objectiveText.text = formattedObjective.ToString().TrimEnd();
        }
        else
        {
            objectiveText.text = "No objectives available.";
        }
    }
}

