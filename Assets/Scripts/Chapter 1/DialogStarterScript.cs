using System;
using UnityEngine;
using TMPro;
using cherrydev;
using Myth_Mystery;
using System.Collections.Generic;
using System.Linq;

namespace DialogNodeBasedSystem.Scripts
{
    public class DialogStarterScript : MonoBehaviour
    {
        [SerializeField] private DialogBehaviour dialogBehaviour;
        [SerializeField] private DialogNodeGraph dialogGraph;
        [SerializeField] private CharacterManager characterManager;

        [Header("Character Info")]
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI charInfo;
        [SerializeField] private List<CharacterData> allCharacters;

        private void Start()
        {
            dialogBehaviour.BindExternalFunction("changeSprite", changeCharacter);
            dialogBehaviour.BindExternalFunction("clear", clearCharacter);

            dialogBehaviour.StartDialog(dialogGraph);
            dialogBehaviour.SentenceEnded += OnSentenceEnded;
        }

        private void OnDestroy()
        {
            if (dialogBehaviour != null)
            {
                dialogBehaviour.SentenceEnded -= OnSentenceEnded;
            }
        }

        private void OnSentenceEnded()
        {
            Debug.Log("OnSentenceEnded event was called.");
            string data = charInfo.text.Trim();
            string[] parts = data.Split('_');
            string position = "middle";

            if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]))
            {
                position = parts[2];
            }
            Debug.Log($"Attempting to stop animation for position: {position}");
            characterManager.StopAnimation(position);
        }

        private void clearCharacter()
        {
            string data = charInfo.text.Trim();
            string[] parts = data.Split('_');

            if (parts.Length > 0)
            {
                characterManager.ClearCharacter(parts[2]);
            }
        }

        private void changeCharacter()
        {
            string data = charInfo.text.Trim();
            string[] parts = data.Split('_');

            string charKey = "";
            string variation = "Neutral";
            string position = "middle";

            if (parts.Length > 0 && !string.IsNullOrEmpty(parts[0]))
            {
                charKey = parts[0];
            }

            if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[1]))
            {
                variation = parts[1];
            }

            if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]))
            {
                position = parts[2];
            }

            if (charKey.ToLower() == "")
            {
                characterManager.ChangeCharacter("none", "", "");
                nameLabel.text = "";
                return;
            }

            CharacterData charData = allCharacters.FirstOrDefault(c => c.characterName == charKey || c.codeName == charKey);

            if (charData != null)
            {
                if (charKey.ToLower() == charData.codeName.ToLower())
                {
                    nameLabel.text = "???";
                }
                else
                {
                    nameLabel.text = charData.characterName;
                }

                characterManager.ChangeCharacter(charData.characterName, variation, position);
            }
            else
            {

                characterManager.ChangeCharacter("none", "", "");
                nameLabel.text = "";
            }
        }


    }
}





