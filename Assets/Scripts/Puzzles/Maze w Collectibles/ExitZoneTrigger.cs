using UnityEngine;

public class ExitZoneTrigger : MonoBehaviour
{
    public GenerateMaze_wCollectibles mazeGen;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ExitZone"))
        {
            if (other.name == "ExitZone")
            {
                Debug.Log("Reached Exit!");
                mazeGen.OnPlayerExit();
            }
        }
    }
}
