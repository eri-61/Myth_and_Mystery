using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Myth_Mystery
{
    public class CharacterManager : MonoBehaviour
    {
        #region Variables
        public List<CharacterData> allCharacters;
        public List<BackgroundData> allBackgrounds;

        [Header("Character Positions")]
        public Transform leftCharacterPosition;
        public Transform rightCharacterPosition;
        public Transform middleCharacterPosition;

        [Header("Background")]
        public GameObject bg;

        private Dictionary<string, GameObject> activeCharacters = new Dictionary<string, GameObject>();
        #endregion

        public void HideMCharacter()
        {
            StopAnimation("middle");

            if (activeCharacters.ContainsKey("middle") && activeCharacters["middle"] != null)
                activeCharacters["middle"].SetActive(false);
        }

        public void ShowMCharacter()
        {
            if (activeCharacters.ContainsKey("middle") && activeCharacters["middle"] != null)
            {
                activeCharacters["middle"].SetActive(true);
                ResetAnimatorState(activeCharacters["middle"]);
            }
        }

        public void HideLRCharacters()
        {
            foreach (var side in new string[] { "left", "right" })
            {
                if (activeCharacters.TryGetValue(side, out GameObject character))
                {
                    if (character != null)
                    {
                        Debug.Log($"Hiding {side} character: {character.name}");
                        character.SetActive(false);
                    }
                    else
                    {
                        Debug.LogWarning($"{side} character is null, removing from dictionary");
                        activeCharacters.Remove(side);
                    }
                }
                else
                {
                    Debug.LogWarning($"{side} key not found in activeCharacters");
                }
            }
        }

        public void ShowLRCharacters()
        {
            if (activeCharacters.ContainsKey("left") && activeCharacters["left"] != null)
            {
                activeCharacters["left"].SetActive(true);
                ResetAnimatorState(activeCharacters["left"]);
            }
            if (activeCharacters.ContainsKey("right") && activeCharacters["right"] != null)
            {
                activeCharacters["right"].SetActive(true);
                ResetAnimatorState(activeCharacters["right"]);
            }
        }

        public void ChangeCharacter(string characterName, string variation, string position, string background)
        {
            string positionKey = position.ToLower();

            // If character already exists, check if it's the same one
            if (activeCharacters.ContainsKey(positionKey) && activeCharacters[positionKey] != null)
            {
                GameObject existing = activeCharacters[positionKey];
                CharacterData existingData = allCharacters.Find(c => c.characterName.ToLower() == characterName.ToLower());
                GameObject expectedPrefab = existingData != null ? GetPrefabVariation(existingData, variation) : null;

                // Same prefab & character
                if (expectedPrefab != null && existing.name.StartsWith(expectedPrefab.name, System.StringComparison.OrdinalIgnoreCase))
                {
                    Animator existingAnimator = existing.GetComponent<Animator>();
                    if (existingAnimator != null)
                        existingAnimator.SetBool("isTalking", true);

                    return;
                }

                // Otherwise, destroy and replace
                Destroy(existing);
                activeCharacters.Remove(positionKey);
            }

            // Don't spawn if none
            if (string.IsNullOrEmpty(characterName) || characterName == "none")
                return;

            // Change background if needed
            if (!string.IsNullOrEmpty(background) && background.ToLower() != "none")
            {
                BackgroundData bgData = allBackgrounds.Find(x => x.backgroundName.ToLower() == background.ToLower());
                if (bgData != null)
                    bg.GetComponent<Image>().sprite = bgData.bgSprite;
                    //MapManager.Instance.UpdateLocations();
            }

            // Spawn new character
            CharacterData characterData = allCharacters.Find(c => c.characterName.ToLower() == characterName.ToLower());

            if (characterData != null)
            {
                GameObject prefab = GetPrefabVariation(characterData, variation);

                if (prefab != null)
                {
                    Transform targetPosition = GetPositionTransform(position);

                    if (targetPosition != null)
                    {
                        GameObject newCharacter = Instantiate(prefab, targetPosition);
                        newCharacter.transform.SetParent(targetPosition, false);

                        var spriteRenderer = newCharacter.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null)
                        {
                            float spriteHeight = spriteRenderer.bounds.size.y;
                            float worldHeight = Camera.main.orthographicSize * 2f;
                            float scaleFactor = (worldHeight * 0.9f) / spriteHeight;
                            newCharacter.transform.localScale = Vector3.one * scaleFactor;
                        }

                        activeCharacters.Add(positionKey, newCharacter);

                        Animator animator = newCharacter.GetComponent<Animator>();
                        if (animator != null)
                            animator.SetBool("isTalking", true);
                    }
                }
            }
        }

        public void ClearCharacter()
        {
            if (activeCharacters.Count == 0)
                return;

            var lastKey = activeCharacters.Keys.Last();
            if (activeCharacters[lastKey] != null)
                Destroy(activeCharacters[lastKey]);

            activeCharacters.Remove(lastKey);
        }

        public void StopAnimation(string position)
        {
            string positionKey = position.ToLower();
            if (activeCharacters.ContainsKey(positionKey) && activeCharacters[positionKey] != null)
            {
                Animator animator = activeCharacters[positionKey].GetComponent<Animator>();
                if (animator != null)
                    animator.SetBool("isTalking", false);
            }
        }

        private GameObject GetPrefabVariation(CharacterData characterData, string variation)
        {
            switch (variation.ToLower())
            {
                // general
                case "neutral": return characterData.neutralPrefab;
                case "smiling": return characterData.smilingPrefab;
                case "sad": return characterData.sadPrefab;

                // additional
                case "angry": return characterData.angryPrefab;
                case "glad": return characterData.gladPrefab;
                case "worried": return characterData.worriedPrefab;
                case "pensive": return characterData.pensivePrefab;

                // javier
                case "averted": return characterData.avertedPrefab;
                case "dozing": return characterData.dozingOffPrefab;
                case "exhausted": return characterData.exhaustedPrefab;
                case "serious": return characterData.seriousPrefab;
                case "sigh": return characterData.sighPrefab;

                // rafael
                case "flustered": return characterData.flusteredPrefab;
                case "pout": return characterData.poutPrefab;
                case "unamused": return characterData.unamusedPrefab;
                case "smug": return characterData.smugPrefab;

                // anayo
                case "give": return characterData.givePrefab;
                case "whisper": return characterData.whisperPrefab;

                default:
                    return null;
            }
        }

        private Transform GetPositionTransform(string position)
        {
            switch (position.ToLower())
            {
                case "left": return leftCharacterPosition;
                case "right": return rightCharacterPosition;
                case "middle": return middleCharacterPosition;
                default: return null;
            }
        }

        //for showing / hiding characters
        private void ResetAnimatorState(GameObject character)
        {
            Animator animator = character.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Rebind(); 
                animator.Update(0f); 
                animator.SetBool("isTalking", false);
            }
        }
    }
}
