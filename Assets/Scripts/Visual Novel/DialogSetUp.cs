using System.Globalization;
using UnityEngine;

[CreateAssetMenu]
public class DialogTree : ScriptableObject
{
    public DialogSection[] sections;
}

[System.Serializable]
public struct DialogSection
{
    public string[] characterName;
    public string[] dialog;

    public bool endAfterDialog;
    public int nextDialog;

    public BranchPoint branchPoint;
    public Characters[] characters;

    public Sprite[] background;
}

[System.Serializable]
public struct BranchPoint
{
    [TextArea]
    public string question;
    public Answer[] answers;
}

[System.Serializable]
public struct Answer
{
    public string answerLabel;
    public int nextElement;
}

[System.Serializable]
public struct Characters
{
    public string charaName;
    public Transform transform;
    public GameObject characterPrefab;
    public AudioClip voiceClip;

    [HideInInspector] public bool isTalking;
}

public class DialogSetUp : MonoBehaviour
{
    public DialogSection[] sceneDialog;
}

