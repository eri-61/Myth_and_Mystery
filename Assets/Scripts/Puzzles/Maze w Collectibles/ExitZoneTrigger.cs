using UnityEngine;

public class ExitZoneTrigger : MonoBehaviour
{
    // Backwards-compatible field used by the collectibles maze
    public GenerateMaze_wCollectibles mazeGen;

    // Field used by the simple maze implementation
    public GenerateMaze mazeGenSimple;

    private void Awake()
    {
        // If not wired in the Inspector, try to find either generator in the scene.
        if (mazeGen == null && mazeGenSimple == null)
        {
            mazeGen = FindObjectOfType<GenerateMaze_wCollectibles>();
            mazeGenSimple = FindObjectOfType<GenerateMaze>();
            if (mazeGen == null && mazeGenSimple == null)
            {
                Debug.LogWarning("[ExitZoneTrigger] Could not find a maze generator in scene. Assign mazeGen (GenerateMaze_wCollectibles) or mazeGenSimple (GenerateMaze) on this component.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // Accept player by tag or by presence of known player controller components
        bool isPlayer =
            other.CompareTag("Player") ||
            other.GetComponent<MazePlayerController_wCollectibles>() != null ||
            other.GetComponent<MazePlayerController>() != null;

        if (!isPlayer) return;

        Debug.Log("Reached Exit!");
        if (mazeGen != null)
            mazeGen.OnPlayerExit();

        if(mazeGenSimple != null)
            mazeGenSimple.GameWin();

        else
            Debug.LogWarning("[ExitZoneTrigger] mazeGen is null when player entered exit.");
    }
}