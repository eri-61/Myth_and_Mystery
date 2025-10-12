using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isNewGame = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        SceneManager.LoadScene(4);
    }

    public void LoadChapter(int index)
    {
        isNewGame = false;
        SceneManager.LoadScene(index);
    }

    public void LoadSave()
    {
        isNewGame = false;
        //add code
    }
}
