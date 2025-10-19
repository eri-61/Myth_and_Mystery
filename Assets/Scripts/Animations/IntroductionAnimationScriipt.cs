using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
public class IntroductionAnimationScriipt : MonoBehaviour
{
    public float waitTime = 15.5f;
    public float skipButton_waitTime = 2f;
    public int SceneIndex = 1;
    public Button skipButton;

    void Start()
    {
        StartCoroutine(Wait());
        StartCoroutine(WaitForSkip());
        skipButton.onClick.AddListener(SkipIntro);
    }

    IEnumerator WaitForSkip()
    {
        yield return new WaitForSecondsRealtime(skipButton_waitTime);
        skipButton.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(115f);
        skipButton.gameObject.SetActive(false);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSecondsRealtime(waitTime);
        SceneManager.LoadScene(SceneIndex);
    }

    void SkipIntro()
    {
        SceneManager.LoadScene(SceneIndex);
    }
}
