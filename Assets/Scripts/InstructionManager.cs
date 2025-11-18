using UnityEngine;
using UnityEngine.UI;
public class InstructionManager : MonoBehaviour
{
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Time.timeScale = 0f;
    }

    private void OnEnable()
    {
        closeButton.onClick.AddListener(CloseInstructions);
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(CloseInstructions);
    }

    private void CloseInstructions()
    {
        instructionPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
