using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool isNewGame = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartNewGame()
    {
        isNewGame = true;
        SceneManager.LoadScene();
    }

    public void LoadChapter(int index)
    {
        isNewGame = false;
        SceneManager.LoadScene(index);
    }

    public void LoadSave()
    {
        isNewGame = false;
    }
}
