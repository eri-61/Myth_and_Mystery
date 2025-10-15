using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;
    private int previousSceneIndex = -1;
    
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

    private void Start()
    {
        SceneManager.LoadScene(1);
    }
    
    public void LoadScene(int sceneIndex)
    {
 
        previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadAdditiveScene(int sceneIndex)
    {
        previousSceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadAdditiveSceneCoroutine(sceneIndex));

        Scene scene = SceneManager.GetSceneByBuildIndex(previousSceneIndex);
        SceneManager.SetActiveScene(scene);

    }

    private IEnumerator LoadAdditiveSceneCoroutine(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        while (!asyncLoad.isDone) 
        {
            yield return null;
        }
    } 

    public void UnloadScene(int sceneIndex)
    {
        StartCoroutine(UnloadSceneCoroutine(sceneIndex));

    }

    private IEnumerator UnloadSceneCoroutine(int sceneIndex)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneIndex);
        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }
    public void GoBackToPreviousScene()
    {
        if (previousSceneIndex >= 0)
        {
            SceneManager.LoadScene(previousSceneIndex);
        }
        else
        {
            Debug.LogWarning("No previous scene");
        }
    }

    //for map
    public void OpenMapOverlay()
    {
        if (MapManager.Instance != null)
        {
            MapManager.Instance.ShowMap();
        }
        else
        {
            Debug.LogWarning("MapManager not found in scene!");
        }
    }

}
