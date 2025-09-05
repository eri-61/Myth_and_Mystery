using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Transactions;
using System.Security.Cryptography;
using UnityEditor.Experimental.GraphView;


public class OfficeScene01 : MonoBehaviour
{
    public GameObject charJavier;
    public GameObject charRafael;
    public AudioSource BGM;
    public UIOverlayEvents OverlayControl;
    public GameObject LoadBlocker;
    public DialogueSequence dialogueSequence;

    private int currentNodeIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartScene();
        //StartedEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartScene()
    {
        BGM.Play();
        LoadBlocker.SetActive(true);
        StartDialogue();
    }

    public async void StartDialogue()
    {
        LoadBlocker.SetActive(false);
        showNode(currentNodeIndex);

        OverlayControl.SetNextButtonEvent(() => OnNextButtonClicked());
    }

    void OnNextButtonClicked()
    {
        // Increment the node index
        currentNodeIndex++;

        // Check if we have reached the end of the dialogue sequence
        if (currentNodeIndex >= dialogueSequence.Nodes.Count)
        {
            Debug.Log("End of dialogue sequence.");
            OverlayControl.NextButton.interactable = false; // Disable the Next button
            return;
        }

        // Show the next node
        ShowNode(currentNodeIndex);
    }

    void showNode(int nodeIndex)
    {
        if (nodeIndex >= dialogueSequence.Nodes.Count) return;
        var node = dialogueSequence.Nodes[nodeIndex];

        OverlayControl.TalkingChar.text = node.CharacterName;
        OverlayControl.DialogueText.text = node.DialogueText;

        UpdateCharacterExpressions(node);

        if (node.Choices.Count>0)
        {
            showChoices(node);
        }
        else
        {
            OverlayControl.SetNextButtonEvent(() => showNode(currentNodeIndex + 1));
        }
    }

    void showChoices(DialogueNode node)
    {
        OverlayControl.ClearChoices();
        foreach(var choice in node.Choices)
        {
            OverlayControl.AddChoiceButton(choice.ChoiceText, () => ShowNode(choice.NextNodeIndex));
        }
        OverlayControl.NextButton.interactable = false;
    }

    void UpdateCharacterExpressions(DialogueNode node)
    {
        charJavierReset();
        charRafaelReset();
        if (node.CharacterName == "Javier")
        {
            charJavier.transform.Find(node.CharacterExpressions).gameObject.SetActive(true);
        }
        else if (node.CharacterName == "Rafael")
        {
            charRafael.transform.Find(node.CharacterExpressions).gameObject.SetActive(true);
        }
    }
    void charRafaelReset()
    {
        foreach (Transform child in charRafael.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    void charJavierReset()
    {
        foreach (Transform child in charJavier.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Old style of handling event based from the frame count,read out as list/array, and return each frame 
    /// </summary>
    /// <returns></returns>
    IEnumerator StartedEvent()
    {
        yield return new WaitForSeconds(5);
        charRafael.SetActive(true);
    }
}
