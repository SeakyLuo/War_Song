using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserInfo {

    public static int maxWinPerDay = 10;

    public string username;
    public int playerID;
    public List<Collection> collection = new List<Collection>();
    public List<Lineup> lineups = new List<Lineup>();
    public Dictionary<string,int> contracts = new Dictionary<string, int>();
    public List<int> contractCount = new List<int>();
    public int coins = 0;
    public int rank = 0;
    public int lastLineupSelected = -1;
    public int winsToday = 0;
    public Stats total = new Stats();
    public Dictionary<string, Stats> boardResults = new Dictionary<string, Stats>();
    public List<Mission> missions = new List<Mission>();
    public string preferredBoard = "Standard Board";
    public string lastModeSelected = "";
    public int gameID;
    public bool missionSwitched = false;

    public UserInfo(string playerName, int playerId)
    {
        username = playerName;
        playerID = playerId;
        foreach (string contract in Database.contractList)
        {
            contractCount.Add(0);
            contracts.Add(contract, 0);
        }
        foreach (string board in Database.boardList)
            boardResults.Add(board, new Stats());
    }

    public int FindCollection(string name)
    {
        /// return first occurence
        for(int i = 0; i < collection.Count; i++)
            if (collection[i].name == name) return i;
        return -1;
    }
    public int FindCollection(Collection card)
    {
        for (int i = 0; i < collection.Count; i++)
            if (card.Equals(collection[i])) return i;
        return -1;
    }

    public void AddCollection(Collection insert, bool upload = true)
    {
        Collection.InsertCollection(collection, insert);
        if(upload) Upload();
    }
    public void RemoveCollection(int index)
    {
        collection.RemoveAt(index);
        Upload();
    }
    public void RemoveCollection(Collection remove)
    {
        collection.Remove(remove);
        Upload();
    }
    public void ChangeCollectionCount(int index, int deltaAmount)
    {
        if (--collection[index].count == 0) RemoveCollection(index);
        Upload();
    }
    public void ChangeCoins(int deltaAmount)
    {
        coins += deltaAmount;
        Upload();
    }
    public void AddLineup(Lineup lineup)
    {
        lineups.Add(lineup);
        //Upload();
    }
    public void ModifyLineup(Lineup lineup, int index)
    {
        lineups[index] = lineup;
        //Upload();
    }
    public void RemoveLineup(int index)
    {
        lineups.RemoveAt(index);
        //Upload();
    }
    public void SetLastLineupSelected(int index = -1)
    {
        lastLineupSelected = index;
        //  Upload();
    }
    public void SetPreferredBoard(string boardName = "Standard Board")
    {
        preferredBoard = boardName;
        Upload();
    }
    public void SetGameID(int GameId)
    {
        gameID = GameId;
        Upload();
    }
    public void ChangeContracts(string contractName, int deltaAmount)
    {
        contractCount[ContractsManager.contractName.IndexOf(contractName)] += deltaAmount;
        if (contracts.ContainsKey(contractName)) contracts[contractName] += deltaAmount;
        else contracts.Add(contractName, deltaAmount);
        Upload();
    }
    public void ChangeMission(int number)
    {
        missions[number] = new Mission(Database.RandomMission());
        //missionSwitched = true;
        //Upload();
    }
    public void Win()
    {
        total.Win();
        winsToday++;
        Upload();
    }
    public void Lose()
    {
        total.Lose();
        Upload();
    }
    public void Draw()
    {
        total.Draw();
        Upload();
    }

    public void SetContracts() { contracts = new Dictionary<string, int>(); for (int i = 0; i < 5; i++) contracts.Add(ContractsManager.contractName[i],contractCount[i]); }
    public void SetData() { SetContracts(); }
    public string ToJson()
    {
        return "";
    }

    public static string ClassToJson(UserInfo user)
    {
        return JsonUtility.ToJson(user);
    }
    public static UserInfo JsonToClass(string json)
    {
        return JsonUtility.FromJson<UserInfo>(json);
    }
    public void Upload()
    {
        WWWForm infoToPhp = new WWWForm(); //create WWWform to send to php script
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));
        infoToPhp.AddField("userJson", ClassToJson(this));

        WWW sendToPhp = new WWW("http://47.151.234.225/update_userinfo.php", infoToPhp);
        while (!sendToPhp.isDone) { }
    }
    public static UserInfo Download()
    {
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));

        WWW sendToPhp = new WWW("http://47.151.234.225/download_userinfo.php", infoToPhp);

        while (!sendToPhp.isDone) { }
        return JsonToClass(sendToPhp.text);  //sendToPhp.text is the userInfo json file
    }
}

public class CheatAccount:UserInfo
{
    public CheatAccount():base("WarSong CheatAccount", 12345789)
    {
        Collection[] cheat = {  new Collection("Space Witch", "General"), new Collection("Fat Soldier", "Soldier",4),new Collection("Cripple","Cannon",3),new Collection("Dark Bargain", 5),
            new Collection("Soldier Recruitment",5), new Collection("Seek for Advisors"), new Collection("Greeeeeat Elephant","Elephant",3),new Collection("Place a Trap", 5),
            new Collection("Tame an Elephant"),new Collection("Purchase a Horse"), new Collection("King's Guard","Advisor", 3),new Collection("Protect the King", 8),
            new Collection("Monster Hunter","Chariot",4),new Collection("Treasure Horse","Horse",100), new Collection("Space Witch", "General", 2, 20),
            new Collection("Greeeeeat Elephant", "Elephant", 3, 5), new Collection("Wisest Elder", "General"), new Collection("Secret Plan", 3),new Collection("Place a Flag",20),
            new Collection("No Way", 100), new Collection("King of the Dead", "General"), new Collection("The Ore King", "General"),new Collection("Turret","Cannon"),
            new Collection("Link Soldier","Soldier",11), new Collection("Buy 1 Get 1 Free",15), new Collection("Build a Cannon"),new Collection("Betrayal", 5),
            new Collection("Build a Chariot"),new Collection("Winner's Trophy",5),new Collection("Horse Rider","Horse",4),new Collection("Disarm", 11),new Collection("Minesweeper",20)
        };
        foreach (Collection c in cheat) AddCollection(c);
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
                    "Minesweeper","Winner's Trophy","Buy 1 Get 1 Free","Secret Plan","Soldier Recruitment",
                    "No Way","Seek for Advisors","Tame an Elephant","Purchase a Horse","Build a Chariot"
                },
                "Standard Board",
                "CheatLineup",
                "Standard General"
            ),
            new Lineup(
                new Dictionary<Vector2Int, Collection>()
                {
                    {new Vector2Int(4,0), new Collection("Space Witch", "General",1 , 9) },
                    {new Vector2Int(3,0), new Collection("King's Guard","Advisor", 1, 4) },{new Vector2Int(5,0), new Collection("King's Guard","Advisor") },
                    {new Vector2Int(2,0), new Collection("Greeeeeat Elephant", "Elephant") },{new Vector2Int(6,0), new Collection("Greeeeeat Elephant", "Elephant")},
                    {new Vector2Int(1,0), new Collection("Horse Rider","Horse") },{new Vector2Int(7,0), new Collection("Horse Rider","Horse") },
                    {new Vector2Int(0,0), new Collection("Monster Hunter","Chariot") },{new Vector2Int(8,0), new Collection("Monster Hunter","Chariot") },
                    {new Vector2Int(1,2), new Collection("Turret","Cannon") },{new Vector2Int(7,2), new Collection("Turret","Cannon") },
                    {new Vector2Int(0,3), new Collection("Fat Soldier", "Soldier") },{new Vector2Int(2,3), new Collection("Link Soldier","Soldier") },
                    {new Vector2Int(4,3), new Collection("Link Soldier","Soldier") },{new Vector2Int(6,3), new Collection("Link Soldier","Soldier") },{new Vector2Int(8,3), new Collection("Fat Soldier", "Soldier") }
                },
                new List<string>()
                {
                    "Winner's Trophy","Buy 1 Get 1 Free","Place a Trap","Disarm","Place a Flag",
                    "Secret Plan","Soldier Recruitment","No Way", "Protect the King","Betrayal"
                },
                "Standard Board",
                "CheatLineup2",
                "Space Witch"
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
                    "Minesweeper","Winner's Trophy","Buy 1 Get 1 Free","Secret Plan","Soldier Recruitment",
                    "Seek for Advisors","Tame an Elephant","Purchase a Horse","Build a Chariot"
                },
                "Standard Board",
                "CheatLineup2",
                "Space Witch"
            )
        };
        contracts = new Dictionary<string, int>()
        {
            { "Standard Contract", 1111},
            { "Artillery Seller", 111},
            { "Human Resource", 11},
            { "Animal Smuggler", 5},
            { "Wise Elder", 1}
        };
        contractCount = new List<int> { 1111, 111, 11, 5, 1 };
        coins = 99999;
        rank = 9999;
        lastLineupSelected = 1;
        total = new Stats(10, 1, 1);
        preferredBoard = "Standard Board";
    }
}