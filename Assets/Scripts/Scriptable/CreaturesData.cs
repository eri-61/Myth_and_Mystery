using UnityEngine;

[CreateAssetMenu(fileName = "CreaturesData", menuName = "Scriptable Objects/CreaturesData")]
public class CreaturesData : ScriptableObject
{
    public string CreatureName;
    public string shortDescription;
    public string longDescription;
    public Sprite CreatureImage;

    public GameObject CreaturePrefab;

    public int Health;
    public int Attack;

    public bool hasInteracted;
}
