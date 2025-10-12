using UnityEngine;

[CreateAssetMenu(fileName = "CluesData", menuName = "Scriptable Objects/CluesData")]
public class CluesData : ScriptableObject
{
    public Sprite clueIcon;
    public string clueName;
    [TextArea] public string clueDescription;
    public Sprite clueImage;
}
