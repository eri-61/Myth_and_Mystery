using UnityEngine;

public class InstructionManagerMechanics : MonoBehaviour
{
    private const string NewGameKey = "isNewGame";

    public GameOject instructionsPanel;
    public void LoadInstructions()
    {
        bool isNewGame = !PlayerPrefs.HasKey(NewGameKey);

        if (isNewGame)
        {
            instructionsPanel.SetActive(true);

            PlayerPrefs.SetInt(NewGameKey, 0);
            PlayerPrefs.Save();
        }

        else
        {
            instructionsPanel.SetActive(false);
        }
    }
}
