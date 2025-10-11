using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using cherrydev;
using Myth_Mystery;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;

namespace DialogNodeBasedSystem.Scripts
{
    public class DialogStarterScript : MonoBehaviour
    {
        #region Variables
        [Header ("Dialog System")]
        [SerializeField] private DialogBehaviour dialogBehaviour;
        [SerializeField] private DialogNodeGraph dialogGraph;
        [SerializeField] private CharacterManager characterManager;

        [Header("Character Info")]
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI charInfo;
        [SerializeField] private List<CharacterData> allCharacters;

        [Header("Background")]
        [SerializeField] private GameObject bg;
        [SerializeField] private List<BackgroundData> allBg;

        [Header("Animation Video")]
        [SerializeField] private VideoPlayer animationVideo;
        [SerializeField] private GameObject animationUI;
        [SerializeField] private GameObject gUI;

        [Header("Variables")]
        public int sceneIndex = 1;
        public float waitTime = 10f;
        #endregion

        private void Start()
        {
            dialogBehaviour.BindExternalFunction("changeSprite", changeCharacter);
            dialogBehaviour.BindExternalFunction("clear", clearCharacter);
            dialogBehaviour.BindExternalFunction("loadNext", loadNextScene);
            dialogBehaviour.SentenceEnded += OnSentenceEnded;

            StartCoroutine(PlayIntroVideoThenStartDialog());
        }

        IEnumerator PlayIntroVideoThenStartDialog()
        {
            gUI.SetActive(false);
            animationUI.SetActive(true);

            if (animationVideo != null) 
            { 
                animationVideo.Prepare();
                yield return new WaitUntil(() => animationVideo.isPrepared);
                animationVideo.Play();

                yield return new WaitForSeconds(waitTime);
            }

            animationUI.SetActive(false);
            gUI.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            dialogBehaviour.StartDialog(dialogGraph);
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

        void loadNextScene()
        {
            SceneManager.LoadScene(sceneIndex);
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
            Debug.Log($"changeCharacter() called with charInfo.text = '{data}'");
            string[] parts = data.Split('_');

            string charKey = "";
            string variation = "Neutral";
            string position = "middle";
            string background = "";

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

            if (parts.Length >= 4 && !string.IsNullOrEmpty(parts[3]))
            {
                background = parts[3];
            }

            if (charKey.ToLower() == "")
            {
                characterManager.ChangeCharacter("none", "", "","none");
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

                characterManager.ChangeCharacter(charData.characterName, variation, position, background);
            }
            else
            {

                characterManager.ChangeCharacter("none", "", "", "none");
                nameLabel.text = "";
            }
        }


    }
}





