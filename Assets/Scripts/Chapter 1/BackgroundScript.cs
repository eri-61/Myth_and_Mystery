using UnityEngine;
using UnityEngine.UI;

namespace Myth_Mystery
{
    public class BackgroundScript : MonoBehaviour
{
    public Image image;
    public string[] background;

    public void ChangeBackground()
    {
    }

    private void Awake()
    {
        background[0] = "Chapter 1/Background/Ceiling_";
        background[1] = "Chapter 1/Background/Office (Messy & Day)";
    }
}
}

