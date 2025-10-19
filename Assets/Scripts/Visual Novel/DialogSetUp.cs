using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string Name;
    public bool isQuestion;
    [TextArea] public string dialogText;
    public Transform transform;
    public GameObject characterPrefab;
    public AudioClip voiceClip;
}

public class DialogSetUp : MonoBehaviour
{
    public Dialog[] sceneDialogs;
}
