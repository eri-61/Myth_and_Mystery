using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string codeName;

    public GameObject neutralPrefab;
    public GameObject smilingPrefab;
    public GameObject avertedPrefab;
    public GameObject dozingOffPrefab;
    public GameObject exhaustedPrefab;
    public GameObject seriousPrefab;
    public GameObject sighPrefab;
    public GameObject angryPrefab;
    public GameObject flusteredPrefab;
    public GameObject pensivePrefab;
    public GameObject poutPrefab;
    public GameObject unamusedPrefab;
    public GameObject worriedPrefab;
    public GameObject gladPrefab;
    public GameObject givePrefab;
    public GameObject whisperPrefab;
    public GameObject sadPrefab;
}
