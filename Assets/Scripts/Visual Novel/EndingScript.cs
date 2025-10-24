using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class EndingScript : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI endingText;
    [SerializeField] private Button button;

    [SerializeField] private string[] text;

    void Start()
    {
        button.gameObject.SetActive(false);
        StartCoroutine(showEnding());
    }

    private void OnEnable()
    {
        button.onClick.AddListener(BackToMain);
    }

    void OnDisable()
    {
        button.onClick.RemoveListener(BackToMain);
    }
    void BackToMain()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    IEnumerator showEnding()
    {
        yield return new WaitForSecondsRealtime(2f);
        endingText.text = text[0];

        yield return new WaitForSecondsRealtime(1f);
        endingText.text = text[1];

        yield return new WaitForSecondsRealtime(1f);
        endingText.text = text[2];

        yield return new WaitForSecondsRealtime(1f);
        _image.color = Color.white;

        yield return new WaitForSecondsRealtime(2f);
        endingText.text = text[3];

        yield return new WaitForSecondsRealtime(2f);
        button.gameObject.SetActive(true);
    }
}
