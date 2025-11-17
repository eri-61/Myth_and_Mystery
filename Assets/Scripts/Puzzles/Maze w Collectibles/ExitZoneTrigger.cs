using UnityEngine;

public class ExitZoneTrigger : MonoBehaviour
{
    public GenerateMaze_wCollectibles mazeGen;
    public GenerateMaze mazeGenSimple;

    private void Awake()
    {
        if (mazeGen == null && mazeGenSimple == null)
        {
            mazeGen = FindObjectOfType<GenerateMaze_wCollectibles>();
            if (mazeGen == null)
                mazeGenSimple = FindObjectOfType<GenerateMaze>();

            if (mazeGen == null && mazeGenSimple == null)
                Debug.LogWarning("[ExitZoneTrigger] Could not find a maze generator in scene. Assign mazeGen (GenerateMaze_wCollectibles) or mazeGenSimple (GenerateMaze) on this component.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        bool isPlayer =
            other.CompareTag("Player") ||
            other.GetComponent<MazePlayerController_wCollectibles>() != null ||
            other.GetComponent<MazePlayerController>() != null;

        if (!isPlayer) return;

        Debug.Log("Reached Exit!");

        if (mazeGen != null)
        {
            mazeGen.OnPlayerExit();
            return;
        }

        if (mazeGenSimple != null)
        {
            mazeGenSimple.GameWin();
            return;
        }

        // Fallback: try to find generators at runtime
        var genCollect = FindObjectOfType<GenerateMaze_wCollectibles>();
        if (genCollect != null) { genCollect.OnPlayerExit(); return; }
        var genSimple = FindObjectOfType<GenerateMaze>();
        if (genSimple != null) { genSimple.GameWin(); return; }

        Debug.LogWarning("[ExitZoneTrigger] No maze generator assigned when player entered the exit zone.");
    }
}