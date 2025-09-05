using UnityEngine;

namespace Myth&Mystery
{
    public class CharacterScript : MonoBehaviour
    {
        [SerializeField] private GameObject rightChara;
        [SerializeField] private GameObject leftChara;
        [SerializeField] private GameObject middleChara;

        private GameObject currentCharacterSprite;

        public void SwitchSprites(string position)
        {

            if (currentCharacterSprite != null)
            {
                Destroy(currentCharacterSprite);
            }

            GameObject prefab = null;

            switch (position)
            {
                case "right":
                    prefab = rightChara;
                    break;
                case "left":
                    prefab = leftChara;
                    break;
                case "middle":
                    prefab = middleChara;
                    break;
            }

            if (prefab != null)
            {
                currentCharacterSprite = Instantiate(prefab, transform.position, Quaternion.identity, this.transform);
            }
            else
            {
                Debug.LogWarning("Invalid position specified: " + position);
            }
        }
    }
}
