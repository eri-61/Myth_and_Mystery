using System.Collections;
using System.Collections.Generic;
using System.Linq;

using cherrydev;
using Myth_Mystery;

using UnityEngine;
using UnityEngine.Video;
using TMPro;
using System.Runtime.CompilerServices;

namespace DialogNodeBasedSystem.Scripts
{
    public class DialogStarterWIInfo : MonoBehaviour
    {
        #region Variables
        [Header("Dialog System")]
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
        [SerializeField] private VideoPlayer dayTimeAnimation;
        [SerializeField] private GameObject dayTimeUI;

        [SerializeField] private VideoClip newClip;

        [SerializeField] private GameObject Fade;
        [SerializeField] private GameObject gUI;

        [Header("Variables")]
        public int sceneIndex = 1;
        public float waitTime = 10f;

        [Header("Scripts and Data")]
        [SerializeField] private CaseFileScript caseFile;
        [SerializeField] private CluesScript clues;
        [SerializeField] private CreaturesScript creatures;
        public TestimonyData testimonyData;
        public CreaturesData creature;
        #endregion

        private void Start()
        {
            dialogBehaviour.BindExternalFunction("changeSprite", changeCharacter);
            dialogBehaviour.BindExternalFunction("clear", clearCharacter);

            dialogBehaviour.BindExternalFunction("hide", hideLRCharacters);
            dialogBehaviour.BindExternalFunction("show", showLRCharacters);

            dialogBehaviour.BindExternalFunction("hideM", hideMCharacter);
            dialogBehaviour.BindExternalFunction("showM", showMCharacter);

            dialogBehaviour.BindExternalFunction("loadNext", loadNextScene);

            dialogBehaviour.BindExternalFunction("waitForJournal", WaitForJournal);
            dialogBehaviour.BindExternalFunction("playAnimation", PlayAnimation);

            dialogBehaviour.BindExternalFunction("updateJournal", updateJournal);
            dialogBehaviour.BindExternalFunction("addTestimony", addTestimony);
            dialogBehaviour.BindExternalFunction("addCreatures", addTestimony);
            dialogBehaviour.BindExternalFunction("revealObjective", RevealObjective);

            dialogBehaviour.SentenceEnded += OnSentenceEnded;

            StartCoroutine(PlayAnimationThenStartDialog());
        }

        IEnumerator PlayAnimationThenStartDialog()
        {
            gUI.SetActive(false);
            Fade.SetActive(false);
            dayTimeUI.SetActive(true);

            //day time animation
            if (dayTimeUI != null)
            {
                dayTimeAnimation.Play();
                yield return new WaitForSeconds(waitTime);
            }

            Fade.SetActive(true);
            dayTimeUI.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            Fade.SetActive(false);
            gUI.SetActive(true);

            dayTimeAnimation.clip = newClip;
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

        public void PlayAnimation()
        {
            Play();
        }

        public IEnumerator Play()
        {
            gUI.SetActive(false);
            dialogBehaviour.IsActive = false;
            dayTimeUI.SetActive(true);

            dayTimeAnimation.Play();
            yield return new WaitForSeconds(waitTime);

            Fade.SetActive(true);
            yield return new WaitForSeconds(1f);

            dialogBehaviour.IsActive = true;
            Fade.SetActive(false);
            gUI.SetActive(true);
            


        }

        public void loadNextScene()
        {
            SceneController.Instance.LoadScene(sceneIndex);
        }

        public void addTestimony()
        {
            clues.addTestimony(testimonyData);
        }

        public void addCreatures()
        {
            creatures.AddCreature(creature);
        }

        public void updateJournal()
        {
            caseFile.UpdateCaseFileUI();
        }

        public void RevealObjective()
        {
            int objectiveIndex = dialogBehaviour.VariablesHandler.GetVariableValue<int>("ObjectiveIndex");

            caseFile.RevealObjective(objectiveIndex);
        }

        private void WaitForJournal()
        {
            dialogBehaviour.IsActive = false;
            GameManager.Instance.WaitForJournal();

        }

        public void OnJournalClosed()
        {
            dialogBehaviour.IsActive = true;
        }

        //characters
        private void showMCharacter()
        {
            characterManager.ShowMCharacter();
        }

        private void hideMCharacter()
        {
            characterManager.HideMCharacter();
        }

        private void hideLRCharacters()
        {
            characterManager.HideLRCharacters();
        }

        private void showLRCharacters()
        {
            characterManager.ShowLRCharacters();
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
                characterManager.ChangeCharacter("none", "", "", "none");
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
