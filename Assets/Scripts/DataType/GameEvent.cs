using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class GameEvent {

    public Location eventLocation = Location.NoLocation;
    public Location targetLocation = Location.NoLocation;
	public string eventTriggerName = ""; // Who (Piece or tactic) triggers this event
    public string targetTriggerName = ""; // Target name
    public int eventPlayerID = -1;
    public int targetPlayerID = -1;
    public string result = ""; // Piece, Tactic, Trap, Freeze, Move, Kill, Flag
    public int amount = 0;

    private static int height;
    private static int width;

    public GameEvent()
    {
        result = "EndTurn";
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

    public GameEvent(Location EventLocation, int EventPlayerID)
    {
        /// Flag
        result = "Flag";
        eventLocation = EventLocation;
        eventPlayerID = EventPlayerID;
    }

    public GameEvent(Location EventLocation)
    {
        /// RemoveFlag
        result = "RemoveFlag";
        eventLocation = EventLocation;
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
        if (eventLocation != Location.NoLocation) eventLocation = new Location(width - eventLocation.x, height - eventLocation.y);
        if (targetLocation != Location.NoLocation) targetLocation = new Location(width - targetLocation.x, height - targetLocation.y);
    }

    public static void SetBoard(BoardAttributes boardAttributes)
    {
        height = boardAttributes.boardHeight - 1;
        width = boardAttributes.boardWidth - 1;
    }
    public static string ClassToJson(GameEvent gameEvent)
    {
        return JsonConvert.SerializeObject(gameEvent);
    }
    public static GameEvent JsonToClass(string json)
    {
        return JsonConvert.DeserializeObject<GameEvent>(json);
    }
    public void Upload(GameEvent gameEvent)
    {
        WWWForm infoToPhp = new WWWForm(); //create WWWform to send to php script
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));
        infoToPhp.AddField("userJson", ClassToJson(gameEvent));

        WWW sendToPhp = new WWW("http://localhost:8888/update_userinfo.php", infoToPhp);
        while (!sendToPhp.isDone) { }
    }
    public static GameEvent Download(GameEvent gameEvent)
    {
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));
        WWW sendToPhp = new WWW("http://localhost:8888/download_userinfo.php", infoToPhp);

        while (!sendToPhp.isDone) { }
        return JsonToClass(sendToPhp.text);  //sendToPhp.text is the userInfo json file
    }
}
