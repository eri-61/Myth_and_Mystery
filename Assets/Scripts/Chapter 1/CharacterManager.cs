using UnityEngine;
using System.Collections.Generic;

namespace Myth_Mystery
{
    public class CharacterManager : MonoBehaviour
    {
        public List<CharacterData> allCharacters;

        [Header("Character Positions")]
        public Transform leftCharacterPosition;
        public Transform rightCharacterPosition;
        public Transform middleCharacterPosition;

        private GameObject currentCharacter;

        public void ChangeCharacter(string characterName, string variation, string position)
        {
            if (currentCharacter != null)
            {
                Destroy(currentCharacter);
            }

            if (string.IsNullOrEmpty(characterName) || characterName == "none")
            {
                return;
            }

            CharacterData characterData = allCharacters.Find(c  => c.characterName.ToLower() == characterName.ToLower());

            if (characterData == null)
            {
                GameObject prefab = GetPrefabVariation(characterData, variation);

                if (prefab != null)
                {
                    Transform targetPosition = GetPositionTransform(position);
                    if (targetPosition != null)
                    {
                        currentCharacter = Instantiate(prefab, targetPosition.position, Quaternion.identity);
                    }
                    else
                    {
                        Debug.LogWarning($"Position '{position}' not recognized.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Variation '{variation}' not found.");
                }
            }
      
            else
            {
                Debug.LogWarning($"Character '{characterName}' not found.");
            }
        }

        private GameObject GetPrefabVariation(CharacterData characterData, string variation)
        {
            switch (variation.ToLower())
            {
                case "averted": return characterData.avertedPrefab;
                case "dozing": return characterData.dozingOffPrefab;
                case "exhausted": return characterData.exhaustedPrefab;
                case "neutral": return characterData.neutralPrefab;
                case "serious": return characterData.seriousPrefab;
                case "sigh": return characterData.sighPrefab;
                case "smiling": return characterData.smilingPrefab;
                case "angry": return characterData.angryPrefab;
                case "flustered": return characterData.flusteredPrefab;
                case "pensive": return characterData.pensivePrefab;
                case "pout": return characterData.poutPrefab;
                case "unamused": return characterData.unamusedPrefab;
                case "worried": return characterData.worriedPrefab;
                case "glad": return characterData.gladPrefab;
                case "give": return characterData.givePrefab;
                case "whisper": return characterData.whisperPrefab;
                default:
                    Debug.LogWarning($"Variation '{variation}' is not supported.");
                    return null;
            }
        }

        private Transform GetPositionTransform(string position)
        {
            switch (position.ToLower())
            {
                case "left":
                    return leftCharacterPosition;
                case "right":
                    return rightCharacterPosition;
                case "middle":
                    return middleCharacterPosition;
                default:
                    return null;
            }
        }

    }
}
