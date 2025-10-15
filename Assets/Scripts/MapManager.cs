using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Location
{
    public string locationName;
    public int mainSceneIndex;
    public Button mainLocationButton;

    public List<Button> insideLocationButtons;
    public List<Image> insideLocationImages;
    public List<int> insideSceneIndexes; 
}

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("Map Panels")]
    public GameObject mainMap;
    public GameObject insideMap;
    public Button closeButton;
    public Button backButton;

    [Header("Locations")]
    public List<Location> locations;

    [Header("Tracking")]
    public bool[] visitedLocations;
    public bool[] visitedInsideLocations;

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
            return;
        }

        visitedLocations = new bool[locations.Count];
        int insideCount = 0;
        foreach (var loc in locations)
            insideCount += loc.insideLocationButtons.Count;
        visitedInsideLocations = new bool[insideCount];
    }

    private void Start()
    {
        InitializeMap();

        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => mainMap.SetActive(false));

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() =>
        {
            insideMap.SetActive(false);
            mainMap.SetActive(true);
        });
    }

    public void InitializeMap()
    {
        for (int i = 0; i < locations.Count; i++)
        {
            var loc = locations[i];

            loc.mainLocationButton.gameObject.SetActive(visitedLocations[i]);
            loc.mainLocationButton.onClick.RemoveAllListeners();
            loc.mainLocationButton.onClick.AddListener(() => VisitLocation(loc));

            for (int j = 0; j < loc.insideLocationButtons.Count; j++)
            {
                loc.insideLocationButtons[j].gameObject.SetActive(false);
                if (loc.insideLocationImages != null && j < loc.insideLocationImages.Count)
                    loc.insideLocationImages[j].gameObject.SetActive(false);

                int capturedIndex = j;
                loc.insideLocationButtons[j].onClick.RemoveAllListeners();
                loc.insideLocationButtons[j].onClick.AddListener(() => VisitInsideLocation(loc, capturedIndex));
            }
        }

        mainMap.SetActive(false);
        insideMap.SetActive(false);
    }

    public void ShowMap()
    {
        mainMap.SetActive(true);
        insideMap.SetActive(false);
    }

    public void VisitLocation(Location loc)
    {
        int locIndex = locations.IndexOf(loc);
        if (locIndex >= 0) visitedLocations[locIndex] = true;

        if (loc.insideLocationButtons.Count > 0)
        {
            insideMap.SetActive(true);
            mainMap.SetActive(false);
            for (int i = 0; i < loc.insideLocationButtons.Count; i++)
            {
                loc.insideLocationButtons[i].gameObject.SetActive(true);
                if (loc.insideLocationImages != null && i < loc.insideLocationImages.Count)
                    loc.insideLocationImages[i].gameObject.SetActive(true);
            }
        }
        else
        {
            if (loc.mainSceneIndex >= 0)
                SceneController.Instance.LoadScene(loc.mainSceneIndex);
        }
    }

    public void VisitInsideLocation(Location loc, int index)
    {
        int globalIndex = 0;
        for (int i = 0; i < locations.Count; i++)
        {
            if (locations[i] == loc) break;
            globalIndex += locations[i].insideLocationButtons.Count;
        }
        globalIndex += index;

        visitedInsideLocations[globalIndex] = true;

        if (loc.insideSceneIndexes != null && index < loc.insideSceneIndexes.Count)
            SceneController.Instance.LoadScene(loc.insideSceneIndexes[index]);
    }

    public void RevealLocation(int index)
    {
        if (index >= 0 && index < locations.Count)
        {
            visitedLocations[index] = true;
            locations[index].mainLocationButton.gameObject.SetActive(true);
        }
    }
}
