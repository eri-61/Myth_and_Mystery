using UnityEngine;


public class TalkingCharacterModel
{    
    public string CharacterName { get; set; }
    public string CharacterDialogue { get; set; }

    public TalkingCharacterModel(string characterName, string characterDialogue)
    {
        CharacterName = characterName;
        CharacterDialogue = characterDialogue;
    }
}
