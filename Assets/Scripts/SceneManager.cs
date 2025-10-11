using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerHelper : MonoBehaviour
{
    public static SceneManagerHelper Instance;
    private int previousSceneIndex = -1;

    void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPreviousScene()
    {
        previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void GoBackToPreviousScene()
    {
        if(previousSceneIndex>=0)
        {
            SceneManager.LoadScene(previousSceneIndex);
        }
        else
        {
            Debug.LogWarning("No previous scene");
        }
    }
}
