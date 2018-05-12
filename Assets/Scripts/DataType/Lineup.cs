using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lineup
{
    public Dictionary<Vector2Int, Collection> cardLocations;
    public List<Tactic> tactics;
    public string boardName;
    public string lineupName;
    public string general;
    public bool complete;
    public static int tacticLimit = 10;

    public Lineup()
    {
        Clear();
    }

    public Lineup(Dictionary<Vector2Int, Collection> cardLoc, List<string> Tactics, string BoardName = "Standard Board", string LineupName = "Custom Lineup", string General = "Standard General")
    {
        List<Tactic> playerTactics = new List<Tactic>();
        foreach (string tacticName in Tactics) playerTactics.Add(new Tactic(Database.FindTacticAttributes(tacticName)));
        SetInfo(cardLoc, playerTactics, BoardName, LineupName, General);
    }

    public Lineup(Dictionary<Vector2Int, Collection> cardLoc, List<Tactic> Tactics, string BoardName = "Standard Board", string LineupName = "Custom Lineup", string General = "Standard General")
    {
        SetInfo(cardLoc, Tactics, BoardName, LineupName, General);
    }

    public void SetInfo(Dictionary<Vector2Int, Collection> cardLoc, List<Tactic> Tactics, string BoardName = "Standard Board", string LineupName = "Custom Lineup", string General = "Standard General")
    {
        cardLocations = cardLoc;
        tactics = Tactics;
        boardName = BoardName;
        lineupName = LineupName;
        general = General;
        if (tactics.Count == tacticLimit) complete = true;
    }

    public void Clear()
    {
        cardLocations = new Dictionary<Vector2Int, Collection>();
        tactics = new List<Tactic>();
        boardName = "Standard Board";
        lineupName = "Custom Lineup";
        general = "Standard General";
        complete = false;
    }

    public bool IsEmpty()
    {
        return cardLocations.Count == 0 && tactics.Count == 0;
    }
}

public class EnemyLineup: Lineup
{
    public EnemyLineup()
    {
        cardLocations = new Dictionary<Vector2Int, Collection>()
        {
            {new Vector2Int(4,0), Collection.General },
            {new Vector2Int(3,0), Collection.Advisor },{new Vector2Int(5,0), Collection.Advisor },
            {new Vector2Int(2,0), Collection.Elephant },{new Vector2Int(6,0), Collection.Elephant },
            {new Vector2Int(1,0), Collection.Horse },{new Vector2Int(7,0), Collection.Horse },
            {new Vector2Int(0,0), Collection.Chariot },{new Vector2Int(8,0), Collection.Chariot },
            {new Vector2Int(1,2), Collection.Cannon },{new Vector2Int(7,2), Collection.Cannon },
            {new Vector2Int(0,3), Collection.Soldier },{new Vector2Int(2,3), Collection.Soldier },
            {new Vector2Int(4,3), Collection.Soldier },{new Vector2Int(6,3), Collection.Soldier },{new Vector2Int(8,3), Collection.Soldier }
        };
        List<string> enemyTactics = new List<string>()
        {
            "Minesweeper","Winner Trophy","Buy 1 Get 1 Free","Secret Plan","Soldier Recruitment",
            "No Way","Seek for Advisors","Tame an Elephant","Purchase a Horse","Build a Chariot"
        };
        tactics = new List<Tactic>();
        foreach (string tacticName in enemyTactics) tactics.Add(new Tactic(Database.FindTacticAttributes(tacticName)));
    }
}