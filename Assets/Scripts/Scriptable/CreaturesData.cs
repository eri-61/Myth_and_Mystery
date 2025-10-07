using UnityEngine;

[CreateAssetMenu(fileName = "CreaturesData", menuName = "Scriptable Objects/CreaturesData")]
public class CreaturesData : ScriptableObject
{
    public string CreatureName;
    public string shortDescription;
    public string longDescription;
    public Sprite CreatureIcon;
    public Sprite CreatureImage;

    public GameObject CreaturePrefab;

    public bool hasInteracted;
}
