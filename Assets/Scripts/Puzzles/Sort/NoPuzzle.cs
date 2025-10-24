using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoPuzzle : MonoBehaviour
{
    public GameObject bg;
    public int nextScseneIndex = 1;
    void Start()
    {
        StartCoroutine(LoadNext());
    }

    IEnumerator LoadNext()
    {
        yield return new WaitForSecondsRealtime(2f);
        bg.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(nextScseneIndex);

    }
}
