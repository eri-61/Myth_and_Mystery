using UnityEngine;

[CreateAssetMenu(fileName = "CreaturesData", menuName = "Scriptable Objects/CreaturesData")]
public class CreaturesData : ScriptableObject
{
    public string CreatureName;
    [TextArea] public string shortDescription;
    [TextArea] public string longDescription; //for main menu journal more in depth
    public Sprite CreatureIcon;
    public Sprite CreatureImage;

    public GameObject CreaturePrefab;
    public InventoryData weaknessItem;

    public bool hasInteracted;
}
