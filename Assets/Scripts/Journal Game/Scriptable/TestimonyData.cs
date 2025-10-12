using UnityEngine;

[CreateAssetMenu(fileName = "TestimonyData", menuName = "Scriptable Objects/TestimonyData")]
public class TestimonyData : ScriptableObject
{
    public string witnessName;
    [TextArea] public string testimonyText;
    public Sprite witnessPortrait;
    public Sprite testimonyIcon;
}
