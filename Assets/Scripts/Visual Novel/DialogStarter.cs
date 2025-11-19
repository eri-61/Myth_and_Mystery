using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class DialogStarter : MonoBehaviour
{
    #region Variables
    [Header("Dialog Setup")]
    [SerializeField] private DialogTree dialogTree;
    [SerializeField] private int startSection = 0;

    [Header("Animations")]
    [SerializeField] private GameObject background;

    [SerializeField] private bool caseAnimation = false;
    [SerializeField] private GameObject caseUI;
    [SerializeField] private VideoPlayer caseVideo;

    [SerializeField] private bool dayAnimation = false;
    [SerializeField] private GameObject dayUI;
    [SerializeField] private VideoPlayer dayVideo;

    [SerializeField] private GameObject fadeUI;
    [SerializeField] private float fadeDuration = 1f;

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
        StartCoroutine(BeginSequence());
    }

    IEnumerator BeginSequence()
    {
        //play case
        if (caseAnimation)
        {
            yield return new WaitForSecondsRealtime(1f);
            caseUI.SetActive(true);
            caseVideo.Play();
            yield return new WaitForSecondsRealtime((float)caseVideo.clip.length);
            caseVideo.Stop();
        }

        caseUI.SetActive(false);
        dayUI.SetActive(true);
        //play day
        if (dayAnimation)
        {
            yield return new WaitForSecondsRealtime(1f);
            dayVideo.Play();
            yield return new WaitForSecondsRealtime((float)dayVideo.clip.length);
            dayVideo.Stop(); 
        }

        dayUI.SetActive(false);
        fadeUI.SetActive(true);
 
        yield return new WaitForSecondsRealtime(fadeDuration);

        fadeUI.SetActive(false);
        background.SetActive(false);
        DialogController.instance.StartDialog(dialogTree, startSection);
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
