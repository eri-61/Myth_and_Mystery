using System;
using UnityEngine;

public class SaveLoadScript : MonoBehaviour
{
    public void SaveGame() 
    {
        var curGS = SaveManager.Instance.GetGameState();
        if (curGS != null)
        {
            curGS.LastSaveDateTime = DateTime.Now;
            SaveManager.Instance.SaveGameState(SaveManager.Instance.SelectedSaveIndex);
            SaveManager.Instance.PrintGameStatus();
        }
    }

    public void UpdateSaveLoadUI()
    {

    }
    
}
