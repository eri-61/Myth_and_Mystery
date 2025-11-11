using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("References")]
    public GM_Puzzle1 gameManager;  // Drag your GameManager here

    [Header("Spawn Settings")]
    public GameObject[] goodPrefabs;
    public GameObject[] badPrefabs;
    public float spawnRate = 2f;
    public float spawnRangeX = 8f;
    public float spawnHeight = 6f;

    [Header("Difficulty Scaling")]
    public float fallSpeedIncreaseInterval = 10f; // every 10 seconds
    public float fallSpeedIncreaseAmount = 1f;    // how much faster per interval
    public float baseFallSpeed = 3f;              // starting speed (slow)
    public float spawnRateDecrease = 0.2f;        // spawn faster over time
    public float minSpawnRate = 0.5f;

    private float nextSpawn;
    private float difficultyTimer;
    private float currentFallSpeed;

    void Start()
    {
        currentFallSpeed = baseFallSpeed; // start slow
    }

    void Update()
    {
        if (gameManager == null || Time.timeScale == 0f)
            return;

        difficultyTimer += Time.deltaTime;

        // Spawn new objects
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;
            SpawnObject();
        }

        // Every 10s, make the game harder
        if (difficultyTimer >= fallSpeedIncreaseInterval)
        {
            difficultyTimer = 0f;
            IncreaseDifficulty();
        }
    }

    void SpawnObject()
    {
        float randomX = Random.Range(-spawnRangeX, spawnRangeX);
        Vector3 spawnPos = new Vector3(randomX, spawnHeight, 0f);

        GameObject prefab = (Random.value < 0.3f)
            ? goodPrefabs[Random.Range(0, goodPrefabs.Length)]
            : badPrefabs[Random.Range(0, badPrefabs.Length)];

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        // ✅ Set the fall speed of new objects to the current difficulty speed
        FallingObject fall = obj.GetComponent<FallingObject>();
        if (fall != null)
        {
            fall.fallSpeed = currentFallSpeed;
        }
    }

    void IncreaseDifficulty()
    {
        // Faster spawn rate (shorter delay)
        spawnRate = Mathf.Max(minSpawnRate, spawnRate - spawnRateDecrease);

        // Increase global fall speed
        currentFallSpeed += fallSpeedIncreaseAmount;

        // Also increase speed of existing falling objects
        FallingObject[] allFalling = FindObjectsOfType<FallingObject>();
        foreach (var f in allFalling)
        {
            f.fallSpeed = currentFallSpeed;
        }

        Debug.Log($"Increased difficulty! Fall speed = {currentFallSpeed}, Spawn rate = {spawnRate}");
    }
}
