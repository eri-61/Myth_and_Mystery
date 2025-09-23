using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroductionAnimationScriipt : MonoBehaviour
{
    public float waitTime = 15.5f;

    void Start()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(4);
    }
}
