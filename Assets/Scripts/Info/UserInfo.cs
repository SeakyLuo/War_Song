using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class UserInfo {

    public static int maxWinPerDay = 10;
    public static int maxMissions = 5;

    public string username;
    public int playerID;
    public List<Collection> collection = new List<Collection>();
    public List<Lineup> lineups = new List<Lineup>();
    public Dictionary<string,int> contracts = new Dictionary<string, int>();
    public int coins = 0;
    public int rank = 0;
    public int lastLineupSelected = -1;
    public int winsToday = 0;
    public Stats total = new Stats();
    public Dictionary<string, Stats> boardResults = new Dictionary<string, Stats>();
    public List<Mission> missions;
    public string preferredBoard = "Standard Board";
    public string lastModeSelected = "";
    public int gameID;
    public bool missionSwitched = false;

    public UserInfo(string playerName, int playerId)
    {
        username = playerName;
        playerID = playerId;
        foreach (string contract in Database.contractList)
            contracts.Add(contract, 0);
        foreach (string board in Database.boardList)
            boardResults.Add(board, new Stats());
        missions = new List<Mission>();
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
        if (upload) Upload();
    }
    public void RemoveCollection(int index, bool upload = true)
    {
        collection.RemoveAt(index);
        if (upload) Upload();
    }
    public void RemoveCollection(Collection remove, bool upload = true)
    {
        collection.Remove(remove);
        if (upload) Upload();
    }
    public void ChangeCollectionCount(int index, int deltaAmount, bool upload = true)
    {
        if (--collection[index].count == 0) RemoveCollection(index);
        if (upload) Upload();
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
        if (index == lastLineupSelected) return;
        lastLineupSelected = index;
        Upload();
    }
    public void SetPreferredBoard(string boardName = "Standard Board")
    {
        if (preferredBoard == boardName) return;
        preferredBoard = boardName;
        Upload();
    }
    public void SetGameID(int GameID)
    {
        gameID = GameID;
        Upload();
    }
    public void ChangeContracts(string contractName, int deltaAmount, bool upload = true)
    {
        if (contracts.ContainsKey(contractName)) contracts[contractName] += deltaAmount;
        else contracts.Add(contractName, deltaAmount);
        if(upload) Upload();
    }
    public void ChangeMission(int number)
    {
        missions[number] = new Mission(Database.RandomMission());
        missionSwitched = true;
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

    public static string ClassToJson(UserInfo user)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ContractResolver = new DictionaryAsArrayResolver();
        return JsonConvert.SerializeObject(user, settings);
    }
    public static UserInfo JsonToClass(string json)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ContractResolver = new DictionaryAsArrayResolver();
        return JsonConvert.DeserializeObject<UserInfo>(json, settings);
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