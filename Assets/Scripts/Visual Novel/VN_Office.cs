using UnityEngine;
using UnityEngine.UI;

public class VN_Office : MonoBehaviour
{
    #region Variables
    [Header("Dialog Setup")]
    [SerializeField] private DialogTree dialogTree;
    [SerializeField] private int startSection = 0;
/*
    [Header ("Scriptable Objects)")]
    [SerializeField] private CluesData addClue;
    [SerializeField] private ObjectiveData addObjectives;
    [SerializeField] private TestimonyData addTestimony;

    [Header ("Variables")]
    [SerializeField] private int objectiveIndexToReveal = -1;
    [SerializeField] private bool revealObjectiveOnDialogEnd = false;

    [SerializeField] private bool addClueOnDialogEnd = false;
    [SerializeField] private bool addTestimonyOnDialogEnd = false;

    bool clueAdded = false;
    bool testimonyAdded = false;
    bool objectiveRevealed = false;
    bool waitingForThisDialogEnd = false;
*/
    #endregion
    void Start()
    {
        if (DialogController.instance != null && dialogTree != null)
        {
            DialogController.instance.StartDialog(dialogTree, startSection);
            //waitingForThisDialogEnd = true;
        }
        else
        {
            Debug.LogWarning("Dialog Controller / Tree missing");
        }
    }
    /*
    private void OnEnable()
    {
        DialogController.OnDialogEnded += HandleDialogEnded;
    }

    void OnDisable()
    {
        DialogController.OnDialogEnded -= HandleDialogEnded;
    }

    
    void HandleDialogEnded()
    {
        if (!waitingForThisDialogEnd) return;

        // Add clue
        if (addClueOnDialogEnd && !clueAdded && addClue != null)
        {
            CluesScript clues = GameManager.Instance != null ? GameManager.Instance.cluesScript : null;
            if (clues == null) clues = FindObjectOfType<CluesScript>();
            if (clues != null)
            {
                clues.AddClues(addClue);
                clueAdded = true;
            }
            else
            {
                Debug.LogWarning("Could not find CluesScript to add clue.");
            }
        }

        // Add testimony
        if (addTestimonyOnDialogEnd && !testimonyAdded && addTestimony != null)
        {
            CluesScript clues = GameManager.Instance != null ? GameManager.Instance.cluesScript : null;
            if (clues == null) clues = FindObjectOfType<CluesScript>();
            if (clues != null)
            {
                clues.addTestimony(addTestimony);
                testimonyAdded = true;
            }
            else
            {
                Debug.LogWarning("Could not find CluesScript to add testimony.");
            }
        }

        // Reveal objective (by index if provided, otherwise by reference)
        if (revealObjectiveOnDialogEnd && !objectiveRevealed)
        {
            CaseFileScript caseFile = GameManager.Instance != null ? GameManager.Instance.cfScript : null;
            if (caseFile == null) caseFile = FindObjectOfType<CaseFileScript>();
            if (caseFile != null)
            {
                if (objectiveIndexToReveal >= 0)
                {
                    caseFile.RevealObjective(objectiveIndexToReveal);
                    objectiveRevealed = true;
                }
                else if (addObjectives != null)
                {
                    caseFile.RevealObjective(addObjectives);
                    objectiveRevealed = true;
                }
            }
            else
            {
                Debug.LogWarning("[VN_Office] Could not find CaseFileScript to reveal objective.");
            }
        }

        waitingForThisDialogEnd = false;
    }
    */

}
