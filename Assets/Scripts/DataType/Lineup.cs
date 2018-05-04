using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lineup
{
    public Dictionary<Vector2Int, Collection> cardLocations;
    public List<string> tactics;
    public string boardName, lineupName;
    public bool complete;
    public static int tacticLimit = 10;

    public Lineup()
    {
        cardLocations = new Dictionary<Vector2Int, Collection>();
        tactics = new List<string>();
        boardName = "Standard Board";
        lineupName = "Custom Lineup";
        complete = false;
    }

    public Lineup(Dictionary<Vector2Int, Collection> cardLoc, List<string> Tactics, string BoardName, string LineupName)
    {
        SetInfo(cardLoc, Tactics, BoardName, LineupName);
    }

    public void SetInfo(Dictionary<Vector2Int, Collection> cardLoc, List<string> Tactics, string BoardName, string LineupName)
    {
        cardLocations = cardLoc;
        tactics = Tactics;
        boardName = BoardName;
        lineupName = LineupName;
        if (tactics.Count == tacticLimit) complete = true;
    }

    public void Clear()
    {
        cardLocations = new Dictionary<Vector2Int, Collection>();
        tactics = new List<string>();
        boardName = "Standard Board";
        lineupName = "Custom Lineup";
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
    }
}