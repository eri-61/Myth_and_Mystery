using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    private int previousScene;
    
    void Awake()
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

    public void LoadScene(int sceneIndex)
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    public void GoBack()
    {
        SceneManager.LoadScene(previousScene);
    }

}
