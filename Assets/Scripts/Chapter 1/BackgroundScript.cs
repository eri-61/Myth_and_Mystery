using UnityEngine;
using UnityEngine.UI;

namespace Myth_Mystery
{
    public class BackgroundScript : MonoBehaviour
{
    public Image image;
    public string[] background;

    // Change background by index
    public void ChangeBackground(int index)
    {
        if (index >= 0 && index < background.Length)
        {
            var sprite = Resources.Load<Sprite>(background[index]);
            if (sprite != null)
                image.sprite = sprite;
            else
                Debug.LogWarning("Sprite not found: " + background[index]);
        }
        else
        {
            Debug.LogWarning("Background index out of range: " + index);
        }
    }

    // Change background by name
    public void ChangeBackground(string name)
    {
        var sprite = Resources.Load<Sprite>(name);
        if (sprite != null)
            image.sprite = sprite;
        else
            Debug.LogWarning("Sprite not found: " + name);
    }

    private void Awake()
    {
        background[0] = "Chapter 1/Background/Ceiling_";
        background[1] = "Chapter 1/Background/Office (Messy & Day)";
    }
}
}

