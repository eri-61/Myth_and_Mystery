using UnityEngine;

public enum JournalEntryType { Clue, Testimony }
[CreateAssetMenu(fileName = "CluesData", menuName = "Scriptable Objects/CluesData")]
public class CluesData : ScriptableObject
{
    public JournalEntryType entryType = JournalEntryType.Clue;

    public Sprite clueIcon;
    public string clueName;
    [TextArea] public string clueDescription;
    public Sprite clueImage;
}
