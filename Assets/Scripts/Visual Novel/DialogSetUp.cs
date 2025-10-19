using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Dialog
{
    public string Name;
    [TextArea] public string dialogText;

    public bool isQuestion;
    public Choice[] choices;

    public Transform transform;
    public GameObject characterPrefab;
    public AudioClip voiceClip;

    [HideInInspector] public bool isTalking;
}

    [System.Serializable]
    public class Choice
    {
        public string choiceText;
        public int nextDialogIndex;

        public UnityEvent onChoiceSelected;
    }

    public class DialogSetUp : MonoBehaviour
    {
        public Dialog[] sceneDialogs;
    }

