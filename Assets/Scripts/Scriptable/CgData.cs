using UnityEngine;

[CreateAssetMenu(fileName = "CgData", menuName = "Scriptable Objects/CgData")]
public class CgData : ScriptableObject
{
    public string CGName;
    public Sprite CG;

    public bool isUnlocked;
}
