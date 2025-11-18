using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Investigation_Talk : MonoBehaviour
{
    public void AddClueToJournal(CluesData clue)
    {
        JournalManager.instance.cluesScript.AddClues(clue);

        Debug.Log($"InvestigationScene > AddClueToJournal > Add Clue to Game Save Memory");
        var curGS = SaveManager.Instance.GetGameState();
        if (curGS != null)
        {
            if (curGS.Journal == null)
            {
                curGS.Journal = new GameJournal();
            }

            if (curGS.Journal.Clues == null)
            {
                curGS.Journal.Clues = new List<JournalClue>();
            }

            var existingClue = curGS.Journal.Clues.Where(c => c.ClueName == clue.name).FirstOrDefault();
            int index = curGS.Journal.Clues.Count + 1;

            if (existingClue == null && !string.IsNullOrWhiteSpace(clue.name))
            {
                curGS.Journal.Clues.Add(new JournalClue()
                {
                    ClueNumber = index,
                    ClueName = clue.name,
                    ClueDescription = clue.clueDescription
                });
            }

        }

        Debug.Log($"[InvestigationScene] Added testimony: {clue.clueName}");

    }
}
