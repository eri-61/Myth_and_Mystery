using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
public class IntroductionAnimationScriipt : MonoBehaviour
{
    public float waitTime = 15.5f;
    public float skipButton_waitTime = 2f;

    public Button skipButton;

    void Start()
    {
        StartCoroutine(Wait());
        StartCoroutine(WaitForSkip());
        skipButton.onClick.AddListener(SkipIntro);
    }

    IEnumerator WaitForSkip()
    {
        yield return new WaitForSeconds(skipButton_waitTime);
        skipButton.gameObject.SetActive(true);
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(4);
    }

    void SkipIntro()
    {
        SceneManager.LoadScene(4);
    }
}
