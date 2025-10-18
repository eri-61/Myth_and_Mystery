using System.Collections;
using UnityEngine;

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
        SceneController.Instance.LoadScene(nextScseneIndex);

    }
}
