using UnityEngine;

[CreateAssetMenu(fileName = "CaseFileData", menuName = "Scriptable Objects/CaseFileData")]
public class CaseFileData : ScriptableObject
{
    public string caseTitle;
    public Sprite caseImage;

    public bool isGiven;
    public ObjectiveData[] objectives;
    
}
