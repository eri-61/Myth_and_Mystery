using UnityEngine;

public class InstructionManagerMechanics : MonoBehaviour
{

    public GameObject instructionsPanel;
    
    private void Start()
    {
        if(GameManager.instance != null && GameManager.instance.isNewGame)
        {
            instructionsPanel.SetActive(true);
        }
        else
        {
            instructionsPanel.SetActive(false);
        }
    }
}
