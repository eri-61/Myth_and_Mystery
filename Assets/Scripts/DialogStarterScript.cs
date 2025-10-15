using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using Myth_Mystery;
using cherrydev;

namespace DialogNodeBasedSystem.Scripts
{
    public class DialogStarter : MonoBehaviour
    {
        [Header("Dialog System")]
        [SerializeField] private DialogBehaviour dialogBehaviour;
        [SerializeField] private DialogNodeGraph[] dialogGraph;
        [SerializeField] private CharacterManager characterManager;
        [SerializeField] private int dialogGraphIndex = 0;

        [Header("Character Info")]
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI charInfo;
        [SerializeField] private List<CharacterData> allCharacters;

        [Header("Optional UI References")]
        [SerializeField] private GameObject gUI;
        [SerializeField] private GameObject fade;

        [Header("Animations (Optional)")]
        [SerializeField] private bool playCaseIntro = false;
        [SerializeField] private VideoPlayer caseAnimation;
        [SerializeField] private GameObject caseUI;

        [SerializeField] private bool playDayIntro = false;
        [SerializeField] private VideoPlayer dayTimeAnimation;
        [SerializeField] private GameObject dayTimeUI;

        [SerializeField] private VideoClip clip;

        [Header("Scene & Timing")]
        public int sceneIndex = 1;
        public float fadeDuration = 1.0f;
        public float waitTime = 10f;

        [Header("Scripts and Data")]
        [SerializeField] private CaseFileScript caseFile;
        [SerializeField] private CluesScript clues;
        [SerializeField] private CreaturesScript creatures;
        public TestimonyData testimonyData;
        public CreaturesData creature;

        [Header ("Instructions")]
        [SerializeField] private InstructionManagerMechanics instructionManager;
        private void Start()
        {
            // Fallback assignments from GameManager
            if (GameManager.Instance != null)
            {
                if (caseFile == null) caseFile = GameManager.Instance.cfScript;
                if (clues == null) clues = GameManager.Instance.cluesScript;
                if (creatures == null) creatures = GameManager.Instance.creaturesScript;
            }

            BindExternalFunctions();

            dialogBehaviour.SentenceEnded += OnSentenceEnded;

            StartCoroutine(BeginSequence());
        }

        private void OnDestroy()
        {
            if (dialogBehaviour != null)
                dialogBehaviour.SentenceEnded -= OnSentenceEnded;
        }

        private void BindExternalFunctions()
        {
            dialogBehaviour.BindExternalFunction("changeSprite", changeCharacter);

            dialogBehaviour.BindExternalFunction("hide", hideLRCharacters);
            dialogBehaviour.BindExternalFunction("show", showLRCharacters);

            dialogBehaviour.BindExternalFunction("hideM", hideMCharacter);
            dialogBehaviour.BindExternalFunction("showM", showMCharacter);

            dialogBehaviour.BindExternalFunction("loadNext", loadNextScene);

            dialogBehaviour.BindExternalFunction("updateJournal", updateJournal);
            dialogBehaviour.BindExternalFunction("addTestimony", addTestimony);
            dialogBehaviour.BindExternalFunction("addCreatures", addCreatures);
            dialogBehaviour.BindExternalFunction("revealObjective", RevealObjective);

            dialogBehaviour.BindExternalFunction("waitForJournal", WaitForJournal);
            dialogBehaviour.BindExternalFunction("playAnimation", PlayAnimation);

            //instructions
            dialogBehaviour.BindExternalFunction("loadInstructions", ShowInstructions);
        }

        private IEnumerator BeginSequence()
        {
            gUI?.SetActive(false);

            // CASE INTRO
            if (playCaseIntro  == true)
            {
                caseUI.SetActive(true);
                caseAnimation.Play();

                double caseDuration = caseAnimation.clip != null ? caseAnimation.clip.length : waitTime;
                yield return new WaitForSeconds((float)caseDuration);

                caseUI.SetActive(false); 
            }

            // DAYTIME INTRO
            if (playDayIntro == true)
            {
                dayTimeUI.SetActive(true);
                dayTimeAnimation.Play();

                double dayDuration = dayTimeAnimation.clip != null ? dayTimeAnimation.clip.length : waitTime;
                yield return new WaitForSeconds((float)dayDuration);

                dayTimeUI.SetActive(false); 
            }

            // FADE
            if (fade != null)
            {
                fade.SetActive(true);
                yield return new WaitForSeconds(fadeDuration);
                fade.SetActive(false);
            }

            gUI?.SetActive(true);
            dialogBehaviour.StartDialog(dialogGraph[0]);
        }


        private void OnSentenceEnded()
        {
            string data = charInfo.text.Trim();
            string[] parts = data.Split('_');
            string position = parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]) ? parts[2] : "middle";

            characterManager.StopAnimation(position);
        }

        // External Functions

        public void loadNextScene() => SceneController.Instance.LoadScene(sceneIndex);

        public void addTestimony() => clues.addTestimony(testimonyData);
        public void addCreatures() => creatures.AddCreature(creature);
        public void updateJournal() => caseFile?.UpdateCaseFileUI();

        public void RevealObjective()
        {
            int index = dialogBehaviour.VariablesHandler.GetVariableValue<int>("ObjectiveIndex");
            caseFile?.RevealObjective(index);
        }

        private void ShowInstructions()
        {
            instructionManager?.ShowInstructions();
        }

         public void WaitForJournal()
        {
            characterManager.HideLRCharacters();
            characterManager.HideMCharacter();

            GameManager.Instance.WaitForJournal();
        }

        public void OnJournalClosed()
        {
            gUI?.SetActive(true);
            characterManager.ShowLRCharacters();
            characterManager.ShowMCharacter();

        }
        public void PlayAnimation() => StartCoroutine(PlayAnimationRoutine());
        private IEnumerator PlayAnimationRoutine()
        {
            if (dayTimeAnimation == null || dayTimeUI == null)
                yield break;

            if (clip != null)
                dayTimeAnimation.clip = clip;

            dayTimeUI.SetActive(true);
            dayTimeAnimation.Play();

            double videoDuration = dayTimeAnimation.clip != null ? dayTimeAnimation.clip.length : waitTime;
            yield return new WaitForSeconds((float)videoDuration);

            dayTimeUI.SetActive(false);

            if (fade != null)
            {
                fade.SetActive(true);
                yield return new WaitForSeconds(fadeDuration);
                fade.SetActive(false);
            }

            yield return new WaitForSeconds(1f);
            if (dialogGraph != null)
                dialogBehaviour.StartDialog(dialogGraph[dialogGraphIndex + 1]);
        }

        // Characters
        private void showMCharacter() => characterManager.ShowMCharacter();
        private void hideMCharacter() => characterManager.HideMCharacter();
        private void hideLRCharacters() => characterManager.HideLRCharacters();
        private void showLRCharacters() => characterManager.ShowLRCharacters();

        private void changeCharacter()
        {
            string data = charInfo.text.Trim();
            string[] parts = data.Split('_');

            string charKey = parts.ElementAtOrDefault(0) ?? "";
            string variation = parts.ElementAtOrDefault(1) ?? "Neutral";
            string position = parts.ElementAtOrDefault(2) ?? "middle";
            string background = parts.ElementAtOrDefault(3) ?? "";

            if (string.IsNullOrEmpty(charKey))
            {
                characterManager.ChangeCharacter("none", "", "", "none");
                nameLabel.text = "";
                return;
            }

            CharacterData charData = allCharacters.FirstOrDefault(c =>
                c.characterName == charKey || c.codeName == charKey);

            if (charData != null)
            {
                nameLabel.text = (charKey.ToLower() == charData.codeName.ToLower()) ? "???" : charData.characterName;
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
