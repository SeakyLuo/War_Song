using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class GameInfo
{
    public static Dictionary<string, int> newTurnAction = new Dictionary<string, int> { { "move",1 }, {"ability",1}, {"tactic",1} };

    public Dictionary<Location, List<Piece>> castles = new Dictionary<Location, List<Piece>>();
    public Dictionary<Location, Piece> board = new Dictionary<Location, Piece>();
    public Dictionary<Location, Trigger> triggers = new Dictionary<Location, Trigger>();
    public Dictionary<Location, KeyValuePair<string, int>> traps = new Dictionary<Location, KeyValuePair<string, int>>(); // loc and trap name & creator ID
    public Dictionary<Location, int> flags = new Dictionary<Location, int>();  // loc and player ID
    public Dictionary<int, List<Tactic>> unusedTactics;
    public Dictionary<int, List<Tactic>> usedTactics;
    public Dictionary<int, List<Piece>> activePieces;
    public Dictionary<int, List<Piece>> inactivePieces;

    public string boardName;
    public int currentTurn; //player ID
    public int firstPlayer; //player ID
    public int secondPlayer;
    public Dictionary<int, Lineup> lineups;
    public Dictionary<int, int> ores;
    public Dictionary<int, Dictionary<string ,int>> actions;
    public int round = 1;
    public int time = 90;
    public int maxTime = 90;
    public int gameID;
    public bool gameStarts = false;
    public bool gameOver = false;
    public int victory = -1; // -1 if draw, otherwise playerID

    public GameInfo(Lineup playerLineup, int playerID, Lineup enemyLineup, int enemyID)
    {
        SetOrder(playerID, enemyID);
        lineups = new Dictionary<int, Lineup>()
        {
            { playerID, playerLineup },
            { enemyID, enemyLineup }
        };
        ores = new Dictionary<int, int>()
        {
            { playerID, 30 },
            { enemyID, 30 }
        };
        ResetActions();
        SetGameID(1);
        boardName = playerLineup.boardName;
        activePieces = new Dictionary<int, List<Piece>>()
        {
            { playerID, new List<Piece>() },
            { enemyID, new List<Piece>() },
        };
        inactivePieces = new Dictionary<int, List<Piece>>()
        {
            { playerID, new List<Piece>() },
            { enemyID, new List<Piece>() },
        };
        unusedTactics = new Dictionary<int, List<Tactic>>()
        {
            { playerID, playerLineup.tactics },
            { enemyID, enemyLineup.tactics }
        };
        usedTactics = new Dictionary<int, List<Tactic>>()
        {
            { playerID, new List<Tactic>() },
            { enemyID, new List<Tactic>() }
        };
        gameStarts = true;
    }
    public int TheOtherPlayer()
    {
        if (firstPlayer == Login.playerID) return secondPlayer;
        else return firstPlayer;
    }

    public void NextTurn()
    {
        if (currentTurn == firstPlayer) currentTurn = secondPlayer;
        else currentTurn = firstPlayer;
        round++;
        time = maxTime;
        ResetActions();
    }

    public void AddPiece(Trigger trigger, bool reactivate = false, bool upload = true)
    {
        Piece piece = trigger.piece;
        piece.active = true;
        board.Add(piece.GetCastle(), piece);
        triggers.Add(piece.GetCastle(), trigger);
        if (piece.IsAlly())
        {
            activePieces[Login.playerID].Add(piece);
            if (reactivate) inactivePieces[Login.playerID].Remove(piece);
        }
        else
        {
            activePieces[Login.playerID].Add(piece);
            if (reactivate) inactivePieces[Login.playerID].Remove(piece);
        }
        if (castles.ContainsKey(piece.GetCastle())) castles[piece.GetCastle()].Add(piece);
        else castles.Add(piece.GetCastle(), new List<Piece> { piece });
        if(upload) Upload();
    }

    public void RemovePiece(Piece piece, bool upload = true)
    {
        piece.active = false;
        board.Remove(piece.location);
        triggers.Remove(piece.location);
        if (piece.IsAlly())
        {
            activePieces[Login.playerID].Remove(piece);
            inactivePieces[Login.playerID].Add(piece);
        }
        else
        {
            activePieces[TheOtherPlayer()].Remove(piece);
            inactivePieces[TheOtherPlayer()].Add(piece);
        }
        castles[piece.location].Remove(piece);
        if(upload) Upload();
    }

    public void TransformPiece(Location from, Trigger into)
    {
        Piece piece = board[from];
        board[from] = into.piece;
        triggers[from] = into;
        activePieces[piece.ownerID][activePieces[piece.ownerID].IndexOf(piece)] = into.piece;
    }

    public void FreezePiece(Location location, int round)
    {
        Piece piece = board[location];
        int index = activePieces[Login.playerID].IndexOf(piece);
        if (index == -1)
            activePieces[OnEnterGame.gameInfo.TheOtherPlayer()][activePieces[OnEnterGame.gameInfo.TheOtherPlayer()].IndexOf(piece)].freeze = round;
        else
            activePieces[Login.playerID][index].freeze = round;
        board[location].freeze = round;
        triggers[location].piece.freeze = round;
        Upload();
    }

    public void AddTactic(Tactic tactic)
    {
        int index = 0;
        List<Tactic> tactics = unusedTactics[Login.playerID];
        if (tactics.Count == 0 || tactic < tactics[0]) index = 0;
        else if (tactic > tactics[tactics.Count - 1]) index = tactics.Count;
        else
        {
            for (int i = 0; i < unusedTactics[Login.playerID].Count - 1; i++)
            {
                if (tactic > tactics[i] && tactic < tactics[i+1])
                {
                    index = i + 1;
                    break;
                }
            }
        }
        unusedTactics[Login.playerID].Insert(index, tactic);
        Upload();
    }

    public void RemoveTactic(int index)
    {
        usedTactics[Login.playerID].Add(unusedTactics[Login.playerID][index]);
        unusedTactics[Login.playerID].RemoveAt(index);
        Upload();
    }

    public int FindUnusedTactic(string tacticName, int playerID)
    {
        List<Tactic> tactics = unusedTactics[playerID];
        for (int i = 0; i < tactics.Count; i++)
            if (tactics[i].tacticName == tacticName)
                return i;
        return -1;
    }
    public int FindUsedTactic(string tacticName, int playerID)
    {
        List<Tactic> tactics = usedTactics[playerID];
        for (int i = 0; i < tactics.Count; i++)
            if (tactics[i].tacticName == tacticName)
                return i;
        return -1;
    }

    public void SetOrder(int player1, int player2)
    {
        if (Random.Range(1, 2) % 2 == 1)
        {
            firstPlayer = player1;
            secondPlayer = player2;
        }
        else
        {
            firstPlayer = player2;
            secondPlayer = player1;
        }
        Upload();
    }

    public void ResetActions(int playerID = -1)
    {
        if (playerID == -1) actions = new Dictionary<int, Dictionary<string, int>> { { firstPlayer, newTurnAction }, { secondPlayer, newTurnAction } };
        else actions[playerID] = newTurnAction;
    }

    public bool MultiActions(int playerID)
    {
        foreach (var item in actions[playerID])
            if (item.Value > 1)
                return true;
        return false;
    }

    public bool Actable(int playerID)
    {
        if (MultiActions(playerID)) return true;
        foreach (var item in actions[playerID])
            if (item.Value == 0)
                return false;
        return true;
    }

    public void Act(string action, int playerID, int deltaAmount = -1)
    {
        actions[playerID][action] += deltaAmount;
    }

    public void SetGameID(int value)
    {
        gameID = value;
        //firstPlayer.gameID = gameID
        //secondPlayer.gameID = gameID
        Upload();
    }

    public void Move(Location from, Location to)
    {
        Piece piece = board[from];
        board.Remove(from);
        board.Add(to, piece);
        Trigger trigger = triggers[from];
        triggers.Remove(from);
        triggers.Add(to, trigger);
        Upload();
    }

    public void ChangeOre(int playerID, int deltaAmount)
    {
        ores[playerID] += deltaAmount;
        Upload();
    }

    public bool Destroyable(Location location, string destroyer)
    {
        return !triggers[location].cantBeDestroyedBy.Contains(destroyer);  
    }

    public void Clear()
    {
        board.Clear();
        unusedTactics.Clear();
        usedTactics.Clear();
        activePieces[firstPlayer].Clear();
        activePieces[secondPlayer].Clear();
        inactivePieces[firstPlayer].Clear();
        inactivePieces[secondPlayer].Clear();
        round = 1;
    }

    public static string ClassToJson(GameInfo gameInfo)
    {
        return JsonConvert.SerializeObject(gameInfo);
    }
    public static GameInfo JsonToClass(string json)
    {
        return JsonConvert.DeserializeObject<GameInfo>(json);
    }
    public void Upload()
    {
        //WWWForm infoToPhp = new WWWForm(); //create WWWform to send to php script
        //infoToPhp.AddField("email", PlayerPrefs.GetString("email"));
        //infoToPhp.AddField("userJson", ClassToJson(this));

        //WWW sendToPhp = new WWW("http://localhost:8888/update_gameinfo.php", infoToPhp);
        //while (!sendToPhp.isDone) { }
    }
    public static GameInfo Download()
    {
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));

        WWW sendToPhp = new WWW("http://localhost:8888/download_gameinfo.php", infoToPhp);

        while (!sendToPhp.isDone) { }
        return JsonToClass(sendToPhp.text);  //sendToPhp.text is the userInfo json file
    }
}