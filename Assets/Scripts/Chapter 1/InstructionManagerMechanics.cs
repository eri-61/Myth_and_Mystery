using UnityEngine;

public class InstructionManagerMechanics : MonoBehaviour
{

    public GameObject instructionsPanel;
    
    private void Start()
    {
        if(GameManager.Instance != null && GameManager.Instance.isNewGame)
        {
            instructionsPanel.SetActive(true);
        }
        else
        {
            instructionsPanel.SetActive(false);
        }
    }
}
