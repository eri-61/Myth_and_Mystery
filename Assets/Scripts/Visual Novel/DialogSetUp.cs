using UnityEngine;

[CreateAssetMenu]
public class DialogTree : ScriptableObject
{
    public DialogSection[] sections;
}

[System.Serializable]
public struct DialogSection
{
    public string characterName;
    [TextArea] public string dialog;

    public bool endAfterDialog;

    public BranchPoint branchPoint;

    public Transform transform;
    public GameObject characterPrefab;
    public AudioClip voiceClip;

    [HideInInspector] public bool isTalking;
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

public class DialogSetUp : MonoBehaviour
{
    public DialogSection[] sceneDialog;
}

