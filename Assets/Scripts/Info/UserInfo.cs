using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo {

    public string username;
    public List<Collection> collections;
    public List<Lineup> lineups;
    public Dictionary<string,int> contracts;
    public int coins, rank, lastLineupSelected, winsToday;
    public Stats total;
    public Dictionary<string, Stats> boardResult;
    public List<Challenge> challenges;
    public string preferredBoard = "Standard Board";

    public UserInfo()
    {
        username = "WarSong Account";
        collections = new List<Collection> { new Collection("Space Witch", "General"), new Collection("Fat Soldier", "Soldier",4),new Collection("Cripple","Cannon",3),
            new Collection("Soldier Recruitment",5), new Collection("Advisor Recruitment"), new Collection("Greeeeeat Elephant","Elephant",3),
            new Collection("Tame An Elephant"),new Collection("Purchase An Horse"), new Collection("King Guardian","Advisor", 3),
            new Collection("Monster Hunter","Chariot",4),new Collection("Treasure Horse","Horse",100)};
        lineups = new List<Lineup>();
        contracts = new Dictionary<string, int>(){
            { "Standard Contract", 0},
            { "Artillery Seller", 0},
            { "Human Resource", 0},
            { "Animal Smuggler", 0},
            { "Wise Elder", 0}
        };
        coins = 0;
        rank = 0;
        lastLineupSelected = -1;
        total = new Stats(0,0,0);
        challenges = new List<Challenge>();
        preferredBoard = "Standard Board";
    }
}

public class CheatAccount:UserInfo
{
    public CheatAccount():base()
    {
        username = "WarSong CheatAccount";
        Collection[] cheat = { new Collection("Greeeeeat Elephant", "Elephant", 3, 5), new Collection("Zhuge Liang", "General"), new Collection("A Secret Plan", 3),
            new Collection("No Way", 100), new Collection("Qin Shihuang", "General"), new Collection("Xiao He", "General"),new Collection("Turret","Cannon"),
             new Collection("Link Soldier","Soldier",11), new Collection("Buy 1 Get 1 Free",15), new Collection("Build A Cannon","Tactic"),
            new Collection("Build A Rook"),new Collection("Winner Trophy",5),new Collection("Horse Rider","Horse",4),new Collection("Minesweeper",20)
        };
        foreach (Collection c in cheat) collections.Add(c);
        lineups = new List<Lineup>()
        {
            new Lineup(
                new Dictionary<Vector2Int, Collection>()
                {
                    {new Vector2Int(4,0), Collection.General },
                    {new Vector2Int(3,0), Collection.Advisor },{new Vector2Int(5,0), Collection.Advisor },
                    {new Vector2Int(2,0), Collection.Elephant },{new Vector2Int(6,0), Collection.Elephant },
                    {new Vector2Int(1,0), Collection.Horse },{new Vector2Int(7,0), Collection.Horse },
                    {new Vector2Int(0,0), Collection.Chariot },{new Vector2Int(8,0), Collection.Chariot },
                    {new Vector2Int(1,2), Collection.Cannon },{new Vector2Int(7,2), Collection.Cannon },
                    {new Vector2Int(0,3), Collection.Soldier },{new Vector2Int(2,3), Collection.Soldier },
                    {new Vector2Int(4,3), Collection.Soldier },{new Vector2Int(6,3), Collection.Soldier },{new Vector2Int(8,3), Collection.Soldier }
                },
                new List<string>()
                {
                    "Tame An Elephant","Purchase An Horse","Advisor Recruitment","Soldier Recruitment","Minesweeper",
                    "A Secret Plan","Buy 1 Get 1 Free","Build A Rook","Winner Trophy","No Way"
                },
                "Standard Board",
                "CheatLineup"
            ),
            new Lineup(
                new Dictionary<Vector2Int, Collection>()
                {
                    {new Vector2Int(4,0), new Collection("Space Witch", "General") },
                    {new Vector2Int(3,0), new Collection("King Guardian","Advisor", 3) },{new Vector2Int(5,0), new Collection("King Guardian","Advisor", 3) },
                    {new Vector2Int(2,0), new Collection("Greeeeeat Elephant", "Elephant") },{new Vector2Int(6,0), new Collection("Greeeeeat Elephant", "Elephant")},
                    {new Vector2Int(1,0), new Collection("Horse Rider","Horse",4) },{new Vector2Int(7,0), new Collection("Horse Rider","Horse",4) },
                    {new Vector2Int(0,0), new Collection("Monster Hunter","Chariot") },{new Vector2Int(8,0), new Collection("Monster Hunter","Chariot") },
                    {new Vector2Int(1,2), new Collection("Turret","Cannon") },{new Vector2Int(7,2), new Collection("Turret","Cannon") },
                    {new Vector2Int(0,3), new Collection("Fat Soldier", "Soldier",4) },{new Vector2Int(2,3), new Collection("Link Soldier","Soldier",11) },
                    {new Vector2Int(4,3), new Collection("Link Soldier","Soldier",11) },{new Vector2Int(6,3), new Collection("Link Soldier","Soldier",11) },{new Vector2Int(8,3), new Collection("Fat Soldier", "Soldier",4) }
                },
                new List<string>()
                {
                    "Minesweeper","Winner Trophy","Buy 1 Get 1 Free","A Secret Plan","Soldier Recruitment",
                    "Advisor Recruitment", "No Way", "Tame An Elephant","Purchase An Horse","Build A Rook"
                },
                "Standard Board",
                "CheatLineup2"
            ),
            new Lineup(
                new Dictionary<Vector2Int, Collection>()
                {
                    {new Vector2Int(4,0), new Collection("Space Witch", "General") },
                    {new Vector2Int(3,0), Collection.Advisor },{new Vector2Int(5,0), Collection.Advisor },
                    {new Vector2Int(2,0), Collection.Elephant },{new Vector2Int(6,0), Collection.Elephant },
                    {new Vector2Int(1,0), Collection.Horse },{new Vector2Int(7,0), Collection.Horse },
                    {new Vector2Int(0,0), Collection.Chariot },{new Vector2Int(8,0), Collection.Chariot },
                    {new Vector2Int(1,2), Collection.Cannon },{new Vector2Int(7,2), Collection.Cannon },
                    {new Vector2Int(0,3), Collection.Soldier },{new Vector2Int(2,3), Collection.Soldier },
                    {new Vector2Int(4,3), Collection.Soldier },{new Vector2Int(6,3), Collection.Soldier },{new Vector2Int(8,3), Collection.Soldier }
                },
                new List<string>()
                {
                    "Minesweeper","Winner Trophy","Buy 1 Get 1 Free","A Secret Plan","Soldier Recruitment",
                    "Advisor Recruitment", "Tame An Elephant","Purchase An Horse","Build A Rook"
                },
                "Standard Board",
                "CheatLineup2"
            )
        };
        contracts = new Dictionary<string, int>()
        {
            { "Standard Contract", 100},
            { "Artillery Seller", 10},
            { "Human Resource", 5},
            { "Animal Smuggler", 2},
            { "Wise Elder", 1}
        };
        coins = 99999;
        rank = 9999;
        lastLineupSelected = -1;
        total = new Stats(10, 1, 1);
        preferredBoard = "Standard Board";
    }
}