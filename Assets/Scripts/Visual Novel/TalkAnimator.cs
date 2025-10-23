using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class TalkAnimator : MonoBehaviour
{
    [Header("Sprite frames (UI Image)")]
    public Sprite[] frames;

    public float frameRate = 8f;

    Image uiImage;
    Coroutine loop;

    void Awake()
    {
        uiImage = GetComponentInChildren<Image>();
    }

    public void SetTalking(bool on)
    {
        if (on)
        {
            if (loop == null && (frames != null && frames.Length > 0))
                loop = StartCoroutine(TalkLoop());
        }
        else
        {
            if (loop != null)
            {
                StopCoroutine(loop);
                loop = null;
            }
        }
    }

    IEnumerator TalkLoop()
    {
        int idx = 0;
        float delay = frameRate > 0f ? 1f / frameRate : 0.125f;
        while (true)
        {
            var sprite = frames.Length > 0 ? frames[idx % frames.Length] : null;
            if (uiImage != null)
                uiImage.sprite = sprite;

            idx++;
            yield return new WaitForSeconds(delay);
        }
    }
}