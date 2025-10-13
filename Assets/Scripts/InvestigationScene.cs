using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections;
using System.Collections.Generic;

public class InvestigationScene : MonoBehaviour
{
    #region Variables
    [Header("Button")]
    [SerializeField] private GameObject buttons;
    [SerializeField] private Button talk;
    [SerializeField] private Button investigate;

    [Header("Text")]
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI description;

    [Header("Variables")]
    public bool characterExists = true;
    public int talkSceneIndex = 2;
    public float dialogHide = 5f;
    public bool hasBeenClicked = false;
    
    [Header("Investigation Points")]
    public Button[] investigationPoints;
    public string[] text;

    [Header("Items")]
    public InventoryData[] items;

    [Header("Clues")]
    public CluesData[] clues;

    [HideInInspector]private Dictionary<int, bool> isPointSearched = new Dictionary<int, bool>();
    #endregion
    void Start()
    {
        buttons.SetActive(true);
        description.text = "Objective: Investigate the Tent to find clues.";
        dialogPanel.SetActive(true);
        
        if (!characterExists)
        {
            character.SetActive(false);
        }
        else
        {
            character.SetActive(true);
        }

        for (int i = 0; i < investigationPoints.Length; i++)
        {
            isPointSearched.Add(i, false);
        }
    }

    void OnEnable()
    {
        for (int i = 0; i < investigationPoints.Length; i++)
        {
            int index = i;
            investigationPoints[i].onClick.AddListener(() => ClickInvestigationPoints(index));
        }
        talk.onClick.AddListener(talkToCharacter);
        investigate.onClick.AddListener(investigateScene);
    }

    void OnDisable()
    {
        for (int i = 0; i < investigationPoints.Length; i++)
        {
            investigationPoints[i].onClick.RemoveAllListeners() ;
        }

        talk.onClick.RemoveListener(talkToCharacter);
        investigate.onClick.RemoveListener(investigateScene);
    }

    void talkToCharacter()
    {
        if (!characterExists)
        {
            dialogPanel.SetActive(true);
            description.text = "There is no character to talk to here.";
            StartCoroutine(HideDialog(dialogHide));
        }
        else
        {
            SceneController.Instance.LoadScene(talkSceneIndex);
        }
    }

    void investigateScene()
    {
        buttons.SetActive(false);
        character.SetActive(false);
        dialogPanel.SetActive(true);
        description.text = "Investigate the scene!";
        StartCoroutine(HideDialog(dialogHide));
    }

    void ClickInvestigationPoints(int i)
    {
        dialogPanel.SetActive(true);
        if(isPointSearched.ContainsKey(i) && isPointSearched[i])
        {
            description.text = "You've already searched this area.";
        }
        else
        {
            switch (i)
            {
                case 0:
                    description.text = text[i];
                    AddClueToJournal(clues[i]);
                    isPointSearched[i] = true;
                    break;
                case 1:
                    description.text = text[i];
                    AddClueToJournal(clues[i]);
                    isPointSearched[i] = true;
                    break;
                case 2:
                    description.text = text[i];
                    AddClueToJournal(clues[i]);
                    isPointSearched[i] = true;
                    break;
                case 3:
                    description.text = text[i];
                    AddClueToJournal(clues[i]);
                    isPointSearched[i] = true;
                    break;
                default: 
                    description.text = "You found nothing of interest here.";
                    isPointSearched[i] = true;
                    break;

            }
        }
        StartCoroutine(HideDialog(dialogHide));
    }

    void AddClueToJournal(CluesData clue)
    {

        GameManager.Instance.cluesScript.AddClues(clue);
    }

    void AddToInventory(InventoryData newItem)
    {

        GameManager.Instance.AddItem(newItem);
    }

    IEnumerator HideDialog(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialogPanel.SetActive(false);
    }
}
