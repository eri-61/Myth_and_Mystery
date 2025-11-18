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
    public string ItemDescription { get; set; }
}

public class GameInventory
{
    public List<GameItem> Items { get; set; }
}

public class  JournalCase
{
    public int CaseFile { get; set; }
    public List<string> Objectives { get; set; }
}

public class JournalClue
{
    public int ClueNumber { get; set; }
    public string ClueName { get; set; }
    public string ClueDescription { get; set; }
}

public class JournalCreatures
{
    public int CreatureID { get; set; }
    public string CreatureName { get; set; }
    public string CreatureDescription { get; set; }
    public string AdditionalNotes { get; set; }
}

public class GameJournal
{
    public List<JournalCreatures> Creatures { get; set; }
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
