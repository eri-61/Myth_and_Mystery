using System;
using System.Collections.Generic;

public class GameState
{
    public List<GameSave> PlayerSaves { get; set; }
}

public class GameItem
{
    public int ItemIndex { get; set; }
    public string ItemName { get; set; }
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
    public int ClueNumber;
    public string ClueDescription;
}

public class GameJournal
{
    public List<JournalCase> CaseFile { get; set; }
    public List<JournalClue> Clues { get; set; }
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
