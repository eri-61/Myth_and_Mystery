using UnityEngine;
public class ExitZoneTrigger : MonoBehaviour
{
    public GenerateMaze_wCollectibles mazeGen;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // Accept either the "Player" tag or the presence of the player controller component
        if (other.CompareTag("Player") || other.GetComponent<MazePlayerController_wCollectibles>() != null)
        {
            Debug.Log("Reached Exit!");
            if (mazeGen != null)
                mazeGen.OnPlayerExit();
            else
                Debug.LogWarning("[ExitZoneTrigger] mazeGen reference is null. Assign it on the prefab or when instantiating.");
        }
    }
}