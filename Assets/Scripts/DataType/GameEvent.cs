using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent {

    public Vector2Int eventLocation = Piece.noLocation;
    public Vector2Int targetLocation = Piece.noLocation;
	public string eventTriggerName = ""; // Who (Piece or tactic) triggers this event
    public string targetTriggerName = ""; // Target name
    public int eventPlayerID = -1;
    public int targetPlayerID = -1;
    public string result = ""; // Piece, Tactic, Trap, Freeze, Move, Kill

    public GameEvent() { }

    public GameEvent(Vector2Int from, Vector2Int to, int playerID)
    {
        /// Move
        result = "Move";
        eventLocation = from;
        targetLocation = to;
        eventPlayerID = playerID;
    }

    public GameEvent(Piece piece, string Result = "Ability")
    {
        result = Result;
        eventTriggerName = piece.GetName();
        eventPlayerID = piece.ownerID;
    }

    public GameEvent(Tactic tactic)
    {
        result = "Tactic";
        eventTriggerName = tactic.tacticName;
        eventPlayerID = tactic.ownerID;
    }

    public GameEvent(string Result, string TriggerName, int playerID)
    {
        result = Result;
        eventTriggerName = TriggerName;
        eventPlayerID = playerID;
    }

    public GameEvent(string Result, string EventTriggerName, string TargetTriggerName, Vector2Int EventLocation, Vector2Int TargetLocation, int EventPlayerID, int TargetPlayerID)
    {
        result = Result;
        eventTriggerName = EventTriggerName;
        targetTriggerName = TargetTriggerName;
        eventLocation = EventLocation;
        targetLocation = TargetLocation;
        eventPlayerID = EventPlayerID;
        targetPlayerID = TargetPlayerID;
    }

    public GameEvent(Piece eventPiece, Piece targetPiece)
    {
        result = "Piece";
        eventTriggerName = eventPiece.GetName();
        targetTriggerName = targetPiece.GetName();
        eventLocation = eventPiece.location;
        targetLocation = targetPiece.location;
        eventPlayerID = eventPiece.ownerID;
        targetPlayerID = targetPiece.ownerID;
    }

    public void ReceiveEvent(GameEvent gameEvent)
    {
        result = gameEvent.result;
        eventTriggerName = gameEvent.eventTriggerName;
        targetTriggerName = gameEvent.targetTriggerName;
        eventLocation = gameEvent.eventLocation;
        targetLocation = gameEvent.targetLocation;
        eventPlayerID = gameEvent.eventPlayerID;
        targetPlayerID = gameEvent.targetPlayerID;
    }

    public static string ClassToJson(GameEvent gameEvent)
    {
        return JsonUtility.ToJson(gameEvent);
    }
    public static GameEvent JsonToClass(string json)
    {
        return JsonUtility.FromJson<GameEvent>(json);
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
