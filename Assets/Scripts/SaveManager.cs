using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
public class SaveManager
{
    public static SaveManager Instance { get; private set; }

    public GameState CurrentGameState { get; private set; }
    public GameSave CurrentGameSave { get; private set; }

    public int SelectedSaveIndex { get; private set; }

    public SaveManager()
    {
        SaveManager.Instance = this;
    }

    public void InitGameStates()
    {
        Debug.Log($"SaveManager > InitGameStates started");
        var gameSavesFolderPath = $"{Application.persistentDataPath}/GameSaves";
        var freshInstall = false;

        if (!Directory.Exists(gameSavesFolderPath))
        {
            Directory.CreateDirectory(gameSavesFolderPath);
            freshInstall = true;
        }

        if (freshInstall)
        {
            //For Fresh Install
            CurrentGameState = new GameState()
            {
                PlayerSaves = new List<GameSave>()
            };

            SelectedSaveIndex = 0;
        }
        else
        {
            CurrentGameState = new GameState()
            {
                PlayerSaves = new List<GameSave>()
            };

            SelectedSaveIndex = PlayerPrefs.GetInt("CurrentGameSaveIndex");

            foreach (var fil in Directory.GetFiles(gameSavesFolderPath))
            {
                var dataraw = File.ReadAllText(fil);
                var dataTrans = JsonConvert.DeserializeObject<GameSave>(dataraw);
                //Light load (not full load), for UI only
                CurrentGameState.PlayerSaves.Add(dataTrans);
            }
        }

        //if (CurrentGameState == null)
        //{
        //    CurrentGameState = new GameState()
        //    {
        //        PlayerSaves = new List<GameSave>()
        //    };
        //}        
    }

    public GameSave GetGameState()
    {
        Debug.Log($"SaveManager > GetGameState started");
        SelectedSaveIndex = PlayerPrefs.GetInt("CurrentGameSaveIndex");
        return CurrentGameSave;
    }

    public int NewGameState()
    {
        Debug.Log($"SaveManager > NewGameState started");
        CurrentGameSave = new GameSave() { 
            CurrentChapter = 0,
            CurrentDialog = 0,
            CurrentObjective = 0,
            Inventory = new GameInventory(),
            Journal = new GameJournal(),
            Settings= new GameSettings(),
        };

        if (CurrentGameState.PlayerSaves == null)
        {
            CurrentGameState.PlayerSaves = new List<GameSave>();
        }
        CurrentGameState.PlayerSaves.Add(CurrentGameSave);
        SelectedSaveIndex = CurrentGameState.PlayerSaves.Count - 1;
        PlayerPrefs.SetInt("CurrentGameSaveIndex", SelectedSaveIndex);
        return SelectedSaveIndex;
    }


    public void SaveGameState(int SaveIndex)
    {
        Debug.Log($"SaveManager > SaveGameState started");
        CurrentGameState.PlayerSaves[SelectedSaveIndex] = CurrentGameSave;
        var dataTrans = JsonConvert.SerializeObject(CurrentGameSave);
        var gameSaveFilePath = $"{Application.persistentDataPath}/GameSaves/SaveFile{SaveIndex}.txt";
        File.WriteAllText(gameSaveFilePath, dataTrans);
    }

    public void LoadGameState(int SaveIndex)
    {
        Debug.Log($"SaveManager > LoadGameState started");
        var gameSaveFilePath = $"{Application.persistentDataPath}/GameSaves/SaveFile{SaveIndex}.txt";
        var dataraw = File.ReadAllText(gameSaveFilePath);
        var dataTrans = JsonConvert.DeserializeObject<GameSave>(dataraw);
        if(dataTrans != null)
        {
            CurrentGameSave = dataTrans;
        }
    }

    public void PrintGameStatus()
    {
        if (CurrentGameSave != null)
        {
            Debug.Log($"Save Index: {SelectedSaveIndex}, " +
                $"Chapter: {CurrentGameSave.CurrentChapter}," +
                $"Dialog: {CurrentGameSave.CurrentDialog}," +
                $"Object: {CurrentGameSave.CurrentObjective}," +
                $"Date Time: {CurrentGameSave.LastSaveDateTime}," +
                "");
        }
        
    }
}
