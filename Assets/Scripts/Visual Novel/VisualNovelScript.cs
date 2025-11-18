using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class VisualNovelScript : MonoBehaviour
{
    #region Variables

    [Header("Visual Novel Buttons")]
    public Button autoButton;
    public Button hideBtn;

    [Header ("AutoPlay Button")]
    public Image autoButtonImage;
    public Sprite autoPlaySprite;
    public Sprite pauseSprite;

    [Header("Visual Novel Button - Image")]
    public Image hsButton;
    public Sprite hideSprite;
    public Sprite showSprite;

    [Header("Other Buttons")]
    public GameObject buttons;
    public Button menuBtn;
    public Button journalBtn;

    [Header("Panels")]
    public GameObject instructionsPanel;
    public GameObject settingsPanel;
    public GameObject journalPanel;
    public GameObject uiPanel;

    [Header("Close  Panels Button")]
    public Button closeInstructions;

    [Header("Variables")]
    bool isUIHidden = false;
    bool skipAllMode = false;
    bool isAutoPlaying = false;

    [Header("CreatureData")]
    public CreaturesData creature;
    #endregion

    private void Start()
    {
        autoButton.onClick.AddListener(Skip);
        hideBtn.onClick.AddListener(HideUI);

        menuBtn.onClick.AddListener(OpenMenu);
        journalBtn.onClick.AddListener(OpenJournal);
    }

    void OnDisable()
    {
        autoButton.onClick.RemoveListener(Skip);
        hideBtn.onClick.RemoveListener(HideUI);

        menuBtn.onClick.RemoveListener(OpenMenu);
        journalBtn.onClick.RemoveListener(OpenJournal);
      
    }

    void ToggleUI()
    {
        isUIHidden = !isUIHidden;

        if(hsButton != null)
        {
            hsButton.sprite = isUIHidden ? showSprite : hideSprite;
        }
    }
    void TogglePlayPause()
    {
        isAutoPlaying = !isAutoPlaying;

        if (autoButton != null)
        {
            autoButtonImage.sprite = isAutoPlaying ? pauseSprite : autoPlaySprite;
        }
    }

    void Skip()
    {
        TogglePlayPause();
        skipAllMode = !skipAllMode;
        if (DialogController.instance != null)
            DialogController.instance.ToggleSkipAll(skipAllMode);
    }

    void HideUI()
    {
        ToggleUI();
        uiPanel.SetActive(!isUIHidden);
        buttons.SetActive(!isUIHidden);
        autoButton.gameObject.SetActive(!isUIHidden);
    }

    void OpenMenu()
    {
       settingsPanel.SetActive(true);
    }

    void OpenJournal()
    {
        JournalManager.instance.OpenJournal();
    }

    public void AddCreatures(CreaturesData newCreature)
    {
        JournalManager.instance.creaturesScript.AddCreature(newCreature);

        Debug.Log($"InvestigationScene > AddClueToJournal > Add Clue to Game Save Memory");
        var curGS = SaveManager.Instance.GetGameState();
        if (curGS != null)
        {
            if (curGS.Journal == null)
            {
                curGS.Journal = new GameJournal();
            }

            if (curGS.Journal.Creatures == null)
            {
                curGS.Journal.Creatures = new List<JournalCreatures>();
            }

            var existingCreature = curGS.Journal.Creatures.Where(c => c.CreatureName == newCreature.name).FirstOrDefault();
            int index = curGS.Journal.Clues.Count + 1;

            if (existingCreature == null && !string.IsNullOrWhiteSpace(newCreature.name))
            {
                curGS.Journal.Creatures.Add(new JournalCreatures()
                {
                    CreatureID = index,
                    CreatureName = newCreature.name,
                    CreatureDescription = newCreature.shortDescription,
                    AdditionalNotes = newCreature.longDescription
                });
            }

        }

        Debug.Log($"[InvestigationScene] Added creature: {newCreature.CreatureName}");

    }
}


