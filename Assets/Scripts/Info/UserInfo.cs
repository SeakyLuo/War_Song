using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo {

    public static int maxWinPerDay = 10;

    public string username;
    public int playerID;
    public List<Collection> collection;
    public List<Lineup> lineups;
    public Dictionary<string,int> contracts;
    public int coins, rank, lastLineupSelected, winsToday;
    public Stats total;
    public Dictionary<string, Stats> boardResult;
    public List<Mission> missions;
    public string preferredBoard = "Standard Board";
    public string lastModeSelected = "";
    public int gameID;

    public UserInfo()
    {
        username = "WarSong Account";
        collection = new List<Collection>();
        lineups = new List<Lineup>();
        contracts = new Dictionary<string, int>(){
            { "Standard Contract", 1},
            { "Artillery Seller", 0},
            { "Human Resource", 0},
            { "Animal Smuggler", 0},
            { "Wise Elder", 0}
        };
        coins = 0;
        rank = 0;
        lastLineupSelected = -1;
        winsToday = 0;
        total = new Stats(0,0,0);
        missions = new List<Mission>();
        preferredBoard = "Standard Board";
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

    public void AddCollection(Collection insert)
    {
        int index = 0;
        if (collection.Count == 0 || insert < collection[0]) index = 0;
        else if (insert > collection[collection.Count - 1]) index = collection.Count;
        else
        {
            for (int i = 0; i < collection.Count - 1; i++)
            {
                if (insert.Equals(collection[i]))
                {
                    collection[i].count += insert.count;
                    Upload();
                    return;
                }
                if (insert > collection[i] && insert < collection[i + 1])
                {
                    index = i + 1;
                    break;
                }
            }
        }
        collection.Insert(index, insert);
        Upload();
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
        if (--InfoLoader.user.collection[index].count == 0) InfoLoader.user.RemoveCollection(index);
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
        Upload();
    }
    public void ModifyLineup(Lineup lineup, int index)
    {
        lineups[index] = lineup;
        Upload();
    }
    public void RemoveLineup(int index)
    {
        lineups.RemoveAt(index);
        Upload();
    }
    public void SetLastLineupSelected(int index = -1)
    {
        lastLineupSelected = index;
        Upload();
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
        if (contracts.ContainsKey(contractName)) contracts[contractName] += deltaAmount;
        else contracts.Add(contractName, deltaAmount);
        Upload();
    }
    public void Win()
    {
        total.Win();
        winsToday++;
    }
    public void Lose()
    {
        total.Lose();
    }
    public void Draw()
    {
        InfoLoader.user.total.Draw();
    }

    public static string ClassToJson(UserInfo user)
    {
        return JsonUtility.ToJson(user);
    }
    public static UserInfo JsonToClass(string json)
    {
        return JsonUtility.FromJson<UserInfo>(json);
    }
    public IEnumerator Upload()
    {
        WWWForm infoToPhp = new WWWForm(); //create WWWform to send to php script
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));
        infoToPhp.AddField("userJson", ClassToJson(this));

        WWW sendToPhp = new WWW("http://localhost:8888/update_userinfo.php", infoToPhp);
        yield return sendToPhp;
    }
    public void Download()
    {
        Download(this);
    }
    private IEnumerator Download(UserInfo user)
    {
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));

        WWW sendToPhp = new WWW("http://localhost:8888/download_userinfo.php", infoToPhp);
        yield return sendToPhp;

        user = JsonToClass(sendToPhp.text);  //sendToPhp.text is the userInfo json file
    }
}

public class CheatAccount:UserInfo
{
    public CheatAccount():base()
    {
        username = "WarSong CheatAccount";
        playerID = 12345789;
        Collection[] cheat = {  new Collection("Space Witch", "General"), new Collection("Fat Soldier", "Soldier",4),new Collection("Cripple","Cannon",3),new Collection("Dark Bargain", 5),
            new Collection("Soldier Recruitment",5), new Collection("Seek for Advisors"), new Collection("Greeeeeat Elephant","Elephant",3),new Collection("Place a Trap", 5),
            new Collection("Tame an Elephant"),new Collection("Purchase a Horse"), new Collection("King's Guardian","Advisor", 3),new Collection("Protect the King", 8),
            new Collection("Monster Hunter","Chariot",4),new Collection("Treasure Horse","Horse",100), new Collection("Space Witch", "General", 2, 20),
            new Collection("Greeeeeat Elephant", "Elephant", 3, 5), new Collection("Zhuge Liang", "General"), new Collection("Secret Plan", 3),new Collection("Place a Flag",20),
            new Collection("No Way", 100), new Collection("King of the Dead", "General"), new Collection("The Ore King", "General"),new Collection("Turret","Cannon"),
            new Collection("Link Soldier","Soldier",11), new Collection("Buy 1 Get 1 Free",15), new Collection("Build a Cannon"),new Collection("Betrayal", 5),
            new Collection("Build a Chariot"),new Collection("Winner Trophy",5),new Collection("Horse Rider","Horse",4),new Collection("Disarm", 11),new Collection("Minesweeper",20)
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
                    "Minesweeper","Winner Trophy","Buy 1 Get 1 Free","Secret Plan","Soldier Recruitment",
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
                    {new Vector2Int(3,0), new Collection("King's Guardian","Advisor", 1, 4) },{new Vector2Int(5,0), new Collection("King's Guardian","Advisor") },
                    {new Vector2Int(2,0), new Collection("Greeeeeat Elephant", "Elephant") },{new Vector2Int(6,0), new Collection("Greeeeeat Elephant", "Elephant")},
                    {new Vector2Int(1,0), new Collection("Horse Rider","Horse") },{new Vector2Int(7,0), new Collection("Horse Rider","Horse") },
                    {new Vector2Int(0,0), new Collection("Monster Hunter","Chariot") },{new Vector2Int(8,0), new Collection("Monster Hunter","Chariot") },
                    {new Vector2Int(1,2), new Collection("Turret","Cannon") },{new Vector2Int(7,2), new Collection("Turret","Cannon") },
                    {new Vector2Int(0,3), new Collection("Fat Soldier", "Soldier") },{new Vector2Int(2,3), new Collection("Link Soldier","Soldier") },
                    {new Vector2Int(4,3), new Collection("Link Soldier","Soldier") },{new Vector2Int(6,3), new Collection("Link Soldier","Soldier") },{new Vector2Int(8,3), new Collection("Fat Soldier", "Soldier") }
                },
                new List<string>()
                {
                    "Winner Trophy","Buy 1 Get 1 Free","Place a Trap","Disarm","Place a Flag",
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
                    "Minesweeper","Winner Trophy","Buy 1 Get 1 Free","Secret Plan","Soldier Recruitment",
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
        coins = 99999;
        rank = 9999;
        lastLineupSelected = 1;
        total = new Stats(10, 1, 1);
        preferredBoard = "Standard Board";
    }
}