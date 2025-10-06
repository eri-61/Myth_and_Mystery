using UnityEngine;

[CreateAssetMenu(fileName = "EndingData", menuName = "Scriptable Objects/EndingData")]
public class EndingData : ScriptableObject
{
    public string GoodOrBad_End;
    public string Title;

    public bool isUnlocked;
    public bool BadEnd;
    public bool GoodEnd;
}
