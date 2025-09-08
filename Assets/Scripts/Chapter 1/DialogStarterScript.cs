using System;
using UnityEngine;
using cherrydev;

namespace DialogNodeBasedSystem.Scripts
{
    public class DialogStarterScript : MonoBehaviour
    {
        [SerializeField] private DialogBehaviour dialogBehaviour;
        [SerializeField] private DialogNodeGraph dialogGraph;

        public GameObject rightChara;
        public GameObject leftChara;
        public GameObject middleChara;

        private GameObject currentCharacterSprite;

        private void Start()
        {
            dialogBehaviour.BindExternalFunction("changeSprite", SwitchSprites);
            dialogBehaviour.StartDialog(dialogGraph);
        }

        public void SwitchSprites(string spriteName, string position)
        {

            if (currentCharacterSprite != null)
            {
                Destroy(currentCharacterSprite);
            }

            GameObject prefab = Resources.Load<GameObject>(spriteName);

            if (prefab == null)
            {
                Debug.LogWarning("Sprite not found: " + spriteName);
                return;
            }

            Transform parentTransform = null;

            switch (position.ToLower())
            {
                case "right":
                    if (rightChara != null)
                        parentTransform = rightChara.transform;
                    break;
                case "left":
                    if (leftChara != null)
                        parentTransform = leftChara.transform;
                    break;
                case "middle":
                    if (middleChara != null)
                        parentTransform = middleChara.transform;
                    break;
            }

            if (parentTransform != null)
            {
                currentCharacterSprite = Instantiate(prefab, parentTransform.position, Quaternion.identity, parentTransform);
            }
            else
            {
                Debug.LogWarning("Invalid position specified: " + position);
            }
        }
    }
}



