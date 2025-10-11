using UnityEngine;
using UnityEngine.UI;
using SceneManagement;

public class CaseFile : MonoBehaviour
{
    #region Variables
    [Header("Journal Buttons")]
    public Button cluesBtn;
    public Button slBtn;
    public Button creaturesBtn;

    [Header ("Case File Description")]
    public Image caseImage;
    public Image additionalImage;
    public TMPRoUGUI descriptionText;
    public TMPRoUGUI extraText;
    public TMProUGUI caseObjective;

    [Header("Close Button")]
    public Button closeBtn;
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        cluesBtn.onClick.AddListener(openClues);
        slBtn.onClick.AddListener(openSL);
        creaturesBtn.onClick.AddListener(openCreatures);
        closeBtn.onClick.AddListener(CloseCaseFile);
    }

    void OnDisable()
    {
        cluesBtn.onClick.RemoveListener(openClues);
        slBtn.onClick.RemoveListener(openSL);
        creaturesBtn.onClick.RemoveListener(openCreatures);
        closeBtn.onClick.RemoveListener(CloseCaseFile);
    }

    void Start() 
    { 
    }
}
