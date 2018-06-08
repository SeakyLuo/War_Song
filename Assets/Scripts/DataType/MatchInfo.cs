using Newtonsoft.Json;

public class MatchInfo {

    public int playerID;
    public string playerName;
    public int rank;
    public Lineup lineup;

    public MatchInfo() { }

    public MatchInfo(MatchInfo matchInfo)
    {
        playerID = matchInfo.playerID;
        playerName = matchInfo.playerName;
        rank = matchInfo.rank;
        lineup = matchInfo.lineup;
    }

    public MatchInfo(UserInfo player, Lineup PlayerLineup)
    {
        playerID = player.playerID;
        playerName = player.username;
        rank = player.rank;
        lineup = PlayerLineup;
    }

    public string ToJson()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ContractResolver = new DictionaryAsArrayResolver();
        return JsonConvert.SerializeObject(this, settings);
    }

    public static MatchInfo ToClass(string json)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.ContractResolver = new DictionaryAsArrayResolver();
        return JsonConvert.DeserializeObject<MatchInfo>(json, settings);
    }
}
