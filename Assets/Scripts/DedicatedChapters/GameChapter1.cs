using System;
using UnityEngine;

public class GameChapter1 : MonoBehaviour
{

    public int CurrentDialog;
    public int CurrentObjective;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if (SaveManager.Instance == null)
        //{
        //    SaveManager sm = new SaveManager();
        //}
        var curGS = SaveManager.Instance.GetGameState();
        if (curGS != null)
        {
            //curGS.LastSaveDateTime = DateTime.Now;
            //SaveManager.Instance.SaveGameState(SaveManager.Instance.SelectedSaveIndex);
            curGS.CurrentChapter = 1;
            curGS.CurrentDialog = CurrentDialog;
            curGS.CurrentObjective = CurrentObjective;
            SaveManager.Instance.PrintGameStatus();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
