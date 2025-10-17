using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Location
{
    public string Name;
    public Button mainLocation;
    public int mainSceneIndex;
    public BackgroundData associatedBackground; 
}

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("Map Panels")]
    public GameObject mainMap;
    public Button closeButton;

    [Header("Tracking")]
    public List<Location> locations;
    public bool[] visitedLocations;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }


    private void Start()
    {
        InitializeMap();

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => mainMap.SetActive(false));
    }

    public void InitializeMap()
    {
        visitedLocations = new bool[locations.Count];

        for (int i = 0; i < locations.Count; i++)
        {
            var loc = locations[i];

            // Initially hide location
            loc.mainLocation.gameObject.SetActive(false);

            // Add click listener to travel
            loc.mainLocation.onClick.RemoveAllListeners();
            loc.mainLocation.onClick.AddListener(() => VisitLocation(loc));

            // If background has been used, reveal it
            if (loc.associatedBackground != null && loc.associatedBackground.hasBeenUsed)
            {
                RevealLocation(i);
            }
        }

        mainMap.SetActive(false);
    }

    public void ShowMap()
    {
        mainMap.SetActive(true);
    }

    public void VisitLocation(Location loc)
    {
        int locIndex = locations.IndexOf(loc);
        if (locIndex >= 0) visitedLocations[locIndex] = true;

        if (loc.mainSceneIndex >= 0)
            SceneController.Instance.LoadScene(loc.mainSceneIndex);
    }

    public void RevealLocation(int index)
    {
        if (index >= 0 && index < locations.Count)
        {
            visitedLocations[index] = true;
            locations[index].mainLocation.gameObject.SetActive(true);
        }
    }

    public void UpdateLocations()
    {
        for (int i = 0; i < locations.Count; i++)
        {
            var loc = locations[i];
            if (loc.associatedBackground != null && loc.associatedBackground.hasBeenUsed)
            {
                RevealLocation(i);
            }
        }
    }

    public void ResetMap()
    {
        for (int i = 0; i < visitedLocations.Length; i++)
            visitedLocations[i] = false;

        foreach (var loc in locations)
            loc.mainLocation.gameObject.SetActive(false);
    }
}
