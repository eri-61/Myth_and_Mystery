using UnityEngine;
using UnityEngine.UI;

public class InstructionManagerMechanics : MonoBehaviour
{
    [Header("Instructions Panel")]
    public GameObject instructionsPanel;
    public Button close;

    private void OnEnable()
    {
        close.onClick.AddListener(HideInstructions);    
    }

    private void OnDisable()
    {
        close.onClick.RemoveAllListeners();
    }

    public void ShowInstructions()
    {
        instructionsPanel?.SetActive(true);
    }

    public void HideInstructions()
    {
        instructionsPanel?.SetActive(false);
    }
}
