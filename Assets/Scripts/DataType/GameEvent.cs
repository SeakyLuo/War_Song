using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class GameEvent {

    public Location eventLocation = new Location();
    public Location targetLocation = new Location();
	public string eventTriggerName = ""; // Who (Piece or tactic) triggers this event
    public string targetTriggerName = ""; // Target name
    public int eventPlayerID = -1;
    public int targetPlayerID = -1;
    public string result = ""; // Piece, Tactic, Trap, Freeze, Move, Kill, Flag
    public int amount = 0;

    private static int width;
    private static int height;

    public GameEvent() { }

    public GameEvent(string Result)
    {
        result = Result;
    }

    public GameEvent(Location from, Location to, int playerID)
    {
        /// Move
        result = "Move";
        eventLocation = from;
        targetLocation = to;
        eventPlayerID = playerID;
    }

    public GameEvent(Piece piece, string Result = "Ability")
    {
        /// Activate Ability that doesn't require targets.
        result = Result;
        eventTriggerName = piece.GetName();
        eventLocation = piece.location;
        eventPlayerID = piece.ownerID;
    }

    public GameEvent(Tactic tactic)
    {
        /// Tactic
        result = "Tactic";
        eventTriggerName = tactic.tacticName;
        eventPlayerID = tactic.ownerID;
    }

    public GameEvent(string Result, Tactic tactic, int Amount = 0)
    {
        /// Discard or TacticOre or TacticGold
        result = Result;
        targetTriggerName = tactic.tacticName;
        targetPlayerID = tactic.ownerID;
        amount = Amount;
    }

    public GameEvent(string Result, Piece piece, int Amount)
    {
        /// Freeze
        result = "Freeze";
        targetLocation = piece.location;
        targetTriggerName = piece.GetName();
        targetPlayerID = piece.ownerID;
    }

    public GameEvent(string Result, string TriggerName, int playerID)
    {
        result = Result;
        eventTriggerName = TriggerName;
        eventPlayerID = playerID;
    }

    public GameEvent(string Result, Location EventLocation, int EventPlayerID)
    {
        /// Flag or RemoveFlag
        result = Result;
        eventLocation = EventLocation;
        eventPlayerID = EventPlayerID;
    }

    public GameEvent(string trapName, int trapOwnerID, Piece piece)
    {
        /// Trap
        result = "Trap";
        eventTriggerName = trapName;
        targetTriggerName = piece.GetName();
        eventPlayerID = trapOwnerID;
        targetPlayerID = piece.ownerID;
    }

    public GameEvent(string Result, Piece eventPiece, Piece targetPiece, int Amount = 0)
    {
        /// Transform or PieceHealth or PieceCost
        result = Result;
        eventTriggerName = eventPiece.GetName();
        targetTriggerName = targetPiece.GetName();
        eventLocation = eventPiece.location;
        targetLocation = targetPiece.location;
        eventPlayerID = eventPiece.ownerID;
        targetPlayerID = targetPiece.ownerID;
        amount = Amount;
    }

    public GameEvent(GameEvent gameEvent)
    {
        result = gameEvent.result;
        eventTriggerName = gameEvent.eventTriggerName;
        targetTriggerName = gameEvent.targetTriggerName;
        eventLocation = gameEvent.eventLocation;
        targetLocation = gameEvent.targetLocation;
        eventPlayerID = gameEvent.eventPlayerID;
        targetPlayerID = gameEvent.targetPlayerID;
    }
    public void FlipLocation()
    {
        if (!eventLocation.IsNull()) eventLocation = new Location(width - eventLocation.x, height - eventLocation.y);
        if (!targetLocation.IsNull()) targetLocation = new Location(width - targetLocation.x, height - targetLocation.y);
    }

    public static void SetBoard(BoardAttributes boardAttributes)
    {
        width = boardAttributes.boardWidth - 1;
        height = boardAttributes.boardHeight - 1;
    }
    public static string ClassToJson(GameEvent gameEvent)
    {
        return JsonConvert.SerializeObject(gameEvent);
    }
    public static GameEvent JsonToClass(string json)
    {
        return JsonConvert.DeserializeObject<GameEvent>(json);
    }
    public void Upload()
    {
        WWWForm infoToPhp = new WWWForm(); //create WWWform to send to php script
        infoToPhp.AddField("gameID", OnEnterGame.gameInfo.gameID);
        infoToPhp.AddField("playerID", Login.playerID);
        infoToPhp.AddField("GameEvent", ClassToJson(this));
        Debug.Log(result);
        WWW sendToPhp = new WWW("http://47.151.234.225/uploadToGameInfo.php", infoToPhp);
        while (!sendToPhp.isDone) { }
    }
    public static GameEvent Download()
    {
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("gameID", OnEnterGame.gameInfo.gameID);
        infoToPhp.AddField("playerID", Login.playerID);
        WWW sendToPhp = new WWW("http://47.151.234.225/deleteGameInfo.php", infoToPhp);
        while (!sendToPhp.isDone) { }
        Debug.Log(sendToPhp.text);
        if (sendToPhp.text == "" || sendToPhp.text.Contains("Warning")) return null;
        return JsonToClass(sendToPhp.text);  //sendToPhp.text is the userInfo json file
    }
}
