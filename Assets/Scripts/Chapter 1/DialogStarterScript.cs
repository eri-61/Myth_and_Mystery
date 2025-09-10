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

        public TextMeshPro name;
        private GameObject currentCharacterSprite;

        private void Start()
        {
            dialogBehaviour.BindExternalFunction("changeSprite", SwitchSprites);
            dialogBehaviour.StartDialog(dialogGraph);
        }

        public void SwitchSprites()
        {
            string cName = name.text;

            switch (cName)
            {
                case "Rafael":
                    ChangeCharacterSprite("Rafael", "right");
                    break;
                case "Javier":
                    ChangeCharacterSprite("Javier", "left");
                    break;
                case "Lola Remy":
                    ChangeCharacterSprite("Lola Remy", "center");
                    break;
                case "???":
                    ChangeCharacterSprite("???", "center");
                    break;
                case "":
                    if (currentCharacterSprite != null)
                    {
                        currentCharacterSprite.SetActive(false);
                    }
                    break;
                default:
                    Debug.LogWarning("Unknown character name: " + cName);
                    break;
            }
        }

        public void ChangeCharacterSprite(string spriteName, string position)
        {
            if (currentCharacterSprite != null)
            {
                currentCharacterSprite.SetActive(false);
            }
            switch (position.ToLower())
            {
                case "left":
                    currentCharacterSprite = leftChara.transform.Find(spriteName)?.gameObject;
                    break;
                case "right":
                    currentCharacterSprite = rightChara.transform.Find(spriteName)?.gameObject;
                    break;
                case "middle":
                    currentCharacterSprite = middleChara.transform.Find(spriteName)?.gameObject;
                    break;
                default:
                    Debug.LogWarning("Unknown position: " + position);
                    return;
            }
            if (currentCharacterSprite != null)
            {
                currentCharacterSprite.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Sprite not found: " + spriteName + " at position: " + position);
            }
        }

    }
}



