using Newtonsoft.Json;

public class MatchInfo {

    public int playerID;
    public string playerName;
    public int rank;
    public Lineup lineup;

    public MatchInfo(UserInfo player, Lineup PlayerLineup)
    {
        playerID = player.playerID;
        playerName = player.username;
        rank = player.rank;
        lineup = PlayerLineup;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static MatchInfo ToClass(string json)
    {
        return JsonConvert.DeserializeObject<MatchInfo>(json);
    }
}
