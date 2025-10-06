using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "AnimationData", menuName = "Scriptable Objects/AnimationData")]
public class AnimationData : ScriptableObject
{
    public string AnimationName;
    public VideoClip animation;

    public bool hasSeen;
}
