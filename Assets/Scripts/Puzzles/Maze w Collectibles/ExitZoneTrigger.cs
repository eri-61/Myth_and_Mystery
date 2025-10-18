using UnityEngine;

public class ExitZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GenerateMaze_wCollectibles maze = FindObjectOfType<GenerateMaze_wCollectibles>();
            if (maze != null)
            {
                maze.OnPlayerExit();
            }
        }
    }
}
