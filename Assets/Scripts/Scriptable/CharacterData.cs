using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string codeName;

    [Header ("Basic")]
    public GameObject neutralPrefab;
    public GameObject smilingPrefab;
    public GameObject sadPrefab;
    public GameObject gladPrefab;

    [Header ("Javier")]
    public GameObject avertedPrefab;
    public GameObject dozingOffPrefab;
    public GameObject exhaustedPrefab;
    public GameObject seriousPrefab;
    public GameObject sighPrefab;

    [Header ("Rafael")]
    public GameObject smugPrefab;
    public GameObject angryPrefab;
    public GameObject flusteredPrefab;
    public GameObject pensivePrefab;
    public GameObject poutPrefab;
    public GameObject unamusedPrefab;
    public GameObject worriedPrefab;

    [Header("Anayo")]
    public GameObject givePrefab;
    public GameObject whisperPrefab;

}
