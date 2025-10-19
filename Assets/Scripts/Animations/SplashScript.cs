using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class SplashScript : MonoBehaviour
{
    public float waitTime = 15.5f;
    public int SceneIndex = 2;

    void Awake()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(SceneIndex);
    }
}
