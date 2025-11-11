using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Visual_Novel
{
    public class DialogBranchHandler : MonoBehaviour
    {
        [SerializeField] public DialogTree dialogTree;

        [SerializeField] public int badEndingSectionIndex = 1;
        [SerializeField] public int badEndingSceneIndex = 1;

        [SerializeField] public int goodEndingSectionIndex = 2;
        [SerializeField] public int goodEndingSceneIndex = 1;

        void OnEnable()
        {
            DialogController.OnDialogEnded += HandleDialogEnded;
        }

        void OnDisable()
        {
            DialogController.OnDialogEnded -= HandleDialogEnded;
        }

        void HandleDialogEnded()
        {
            var dc = DialogController.instance;
            if (dc == null) return;

            int current = dc.currentSectionIndex;

            if (current == badEndingSectionIndex && badEndingSceneIndex >= 0)
            {
                dc.sceneIndex = badEndingSceneIndex;
            }
            else if (current == goodEndingSectionIndex && goodEndingSceneIndex >= 0)
            {
                dc.sceneIndex = goodEndingSceneIndex;
            }
        }
    }
}