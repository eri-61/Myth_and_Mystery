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

        private Dictionary<string, GameObject> activeCharacters = new Dictionary<string, GameObject>();

        public void ChangeCharacter(string characterName, string variation, string position)
        {
            string positionKey = position.ToLower();

            if (activeCharacters.ContainsKey(positionKey) && activeCharacters[positionKey] != null)
            {
                Destroy(activeCharacters[positionKey]);
                activeCharacters.Remove(positionKey);
            } 

            if (string.IsNullOrEmpty(characterName) || characterName == "none")
            {
                return;
            }

            CharacterData characterData = allCharacters.Find(c  => c.characterName.ToLower() == characterName.ToLower());

            if (characterData != null)
            {
                GameObject prefab = GetPrefabVariation(characterData, variation);

                if (prefab != null)
                {
                    Transform targetPosition = GetPositionTransform(position);
                    
                    if (targetPosition != null)
                    {
                        GameObject newCharacter = Instantiate(prefab, targetPosition.position, Quaternion.identity);
                        activeCharacters.Add(positionKey, newCharacter);

                        Animator animator = newCharacter.GetComponentInChildren<Animator>();

                        if (animator != null)
                        {
                            Debug.Log($"Setting 'IsTalking' on {newCharacter.name} at position {positionKey}");
                            animator.SetBool("isTalking", true);
                        }

                        else
                        {
                            Debug.LogError($"No Animator component found on prefab or its children for character '{characterName}'.");
                        }

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

        public void ClearCharacter(string position)
        {
            string positionKey = position.ToLower();
            if (activeCharacters.ContainsKey(positionKey) && activeCharacters[positionKey] != null)
            {
                Destroy(activeCharacters[positionKey]);
                activeCharacters.Remove(positionKey);
            }
        }

        public void StopAnimation(string position)
        {
            string positionKey = position.ToLower();
            if (activeCharacters.ContainsKey(positionKey) && activeCharacters[positionKey] != null)
            {
                Animator animator = activeCharacters[positionKey].GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("isTalking", false);
                }

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
