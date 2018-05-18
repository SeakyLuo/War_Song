using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent {

    public Vector2Int eventLocation = Piece.noLocation;
    public Vector2Int targetLocation = Piece.noLocation;
    public string eventTrigger = ""; // Who triggers this event
    public bool move = false;
    public bool trap = false;
    public bool piece = false; // Piece ability
    public bool tactic = false;

    public GameEvent() { }

    public GameEvent(Vector2Int from, Vector2Int to)
    {
        /// Move
        move = true;
        eventLocation = from;
        targetLocation = to;
    }

    public GameEvent(string eventName, string EventTrigger, Vector2Int EventLocation, Vector2Int TargetLocation)
    {
        if (eventName == "Tactic") tactic = true;
        else if (eventName == "Piece") piece = true;
        else if (eventName == "Trap") trap = true;
        eventTrigger = EventTrigger;
        eventLocation = EventLocation;
        targetLocation = TargetLocation;
    }

    public void ReceiveEvent(string eventName, GameEvent gameEvent)
    {
        if (eventName == "Move") move = true;
        else if (eventName == "Tactic") tactic = true;
        else if (eventName == "Piece") piece = true;
        else if (eventName == "Trap") trap = true;
        eventTrigger = gameEvent.eventTrigger;
        eventLocation = gameEvent.eventLocation;
        targetLocation = gameEvent.targetLocation;
    }

    public static string ClassToJson(GameEvent gameEvent)
    {
        return JsonUtility.ToJson(gameEvent);
    }
    public static GameEvent JsonToClass(string json)
    {
        return JsonUtility.FromJson<GameEvent>(json);
    }
    public void Upload()
    {
        var u = Upload(this);
        while (u.MoveNext()) { }
    }
    private IEnumerator Upload(GameEvent gameEvent)
    {
        WWWForm infoToPhp = new WWWForm(); //create WWWform to send to php script
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));
        infoToPhp.AddField("userJson", ClassToJson(gameEvent));

        WWW sendToPhp = new WWW("http://localhost:8888/update_userinfo.php", infoToPhp);
        yield return sendToPhp;
    }
    public void Download()
    {
        var d = Download(this);
        while (d.MoveNext()) { }
    }
    private IEnumerator Download(GameEvent gameEvent)
    {
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));

        WWW sendToPhp = new WWW("http://localhost:8888/download_userinfo.php", infoToPhp);
        yield return sendToPhp;

        while (!sendToPhp.isDone) { }
        gameEvent = JsonToClass(sendToPhp.text);  //sendToPhp.text is the userInfo json file
    }
}
