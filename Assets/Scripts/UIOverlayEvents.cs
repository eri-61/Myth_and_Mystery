using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;

public class UIOverlayEvents : MonoBehaviour
{
    public RawImage DialogueBackdrop;
    public GameObject InventoryGroup;

    public Button SkipButton;
    public Button LogButton;
    public Button ItemButton;
    public Button MenuButton;

    public Button NextButton;
    private Action CurrentNextButtonEvent;

    public Transform ChoicesContainer;
    public GameObject ChoiceButtonPrefab;

    public TMP_Text TalkingChar;
    public TMP_Text DialogueText;

    public int TextDelay  = 1500;//miliseconds

    public int DialogueIndex  = 0;
    public int ConvoIndex  = 0;//most important number to control frame per sentence
    public bool AutoRun = false;

    public bool IsInventoryVisible = false;


    /// <summary>
    /// Kick starts the dialogue convo, increases ConvoIndex each run
    /// </summary>
    /// <param name="DialogueScript"></param>
    public async Task RunDialogue(List<TalkingCharacterModel> DialogueScript) 
    {
        if(ConvoIndex < DialogueScript.Count)
        {
            await Task.Delay(TextDelay);
            TalkingChar.text = DialogueScript[ConvoIndex].CharacterName;
            DialogueText.text = DialogueScript[ConvoIndex].CharacterDialogue;
            ConvoIndex++;
        }        
    }

    public async Task NextDialogue(List<TalkingCharacterModel> DialogueScript)
    {
        await Task.Delay(TextDelay);
        TalkingChar.text = DialogueScript[ConvoIndex].CharacterName;
        DialogueText.text = DialogueScript[ConvoIndex].CharacterDialogue;
        ConvoIndex++;
    }

    public void SetNextButtonEvent(Action callback)
    {
        CurrentNextButtonEvent = callback;
        //callback?.Invoke();//trigger if not null
    }

    /// <summary>
    /// Prepare stuff needed and show Inventory
    /// </summary>
    public void ShowInventory()
    {
        IsInventoryVisible = !IsInventoryVisible;
        InventoryGroup.SetActive(IsInventoryVisible);
    }

    public void ClearChoices()
    {
        foreach(Transform child in ChoicesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddChoiceButton(string choiceText, Action onClick)
    {
        GameObject newButton = Instantiate(ChoiceButtonPrefab, ChoicesContainer);
        newButton.GetComponentInChildren<Text>().text = choiceText;
        newButton.GetComponent<Button>().onClick.AddListener(() => onClick.Invoke());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Override or append an event from the calling scene
        NextButton.onClick.AddListener(() =>
        {
            CurrentNextButtonEvent?.Invoke();//trigger if not null         
        });

        ItemButton.onClick.AddListener(() => {
            ShowInventory();
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
