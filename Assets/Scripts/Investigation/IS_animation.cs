using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class IS_animation : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject animationUI;
    [SerializeField] private GameObject fade;

    public bool playOnStart = true;
    private void Start()
    {
        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        bool playing = GameManager.Instance.playOnStart;

        if (playing == true)
        {
            GameManager.Instance.playOnStart = false;
            videoPlayer.Play();
            yield return new WaitForSecondsRealtime((float)videoPlayer.clip.length);

            fade.SetActive(true);
            animationUI.SetActive(false);

            yield return new WaitForSecondsRealtime(0.5f);
            fade.SetActive(false);
        }

        GameManager.Instance.playOnStart = false;
        animationUI.SetActive(false);

    }
}
