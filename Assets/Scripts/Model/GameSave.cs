using System;
using System.Collections.Generic;

public class GameState
{
    public List<GameSave> PlayerSaves { get; set; }
}

public class GameItem
{
    public int ItemIndex { get; set; }
    public int ItemName { get; set; }
}

public class GameInventory
{
    public List<GameItem> Items { get; set; }
}

public class  JournalCase
{
    public int CaseFile;
    public List<string> Objectives;
}

public class JournalClue
{
    public string ObjectiveDescription;
    public bool IsCompleted;
}

public class GameJournal
{
    public List<int> CaseNumbers { get; set; }
    public List<string> Objectives { get; set; }
    public List<>
}

public class GameSettings
{

}

public class GameSave
{
    public int CurrentChapter { get; set; }
    public int CurrentDialog { get; set; }
    public int CurrentObjective { get; set; }
    public GameInventory Inventory { get; set; }
    public GameJournal Journal { get; set; }
    public GameSettings Settings { get; set; }
    public DateTime LastSaveDateTime { get; set; }
}
