using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameInfo
{
    public Dictionary<Vector2Int, List<Piece>> castles = new Dictionary<Vector2Int, List<Piece>>();
    public Dictionary<Vector2Int, Piece> board = new Dictionary<Vector2Int, Piece>();
    public Dictionary<Vector2Int, KeyValuePair<string, int>> traps = new Dictionary<Vector2Int, KeyValuePair<string, int>>(); // loc and trap name & creator ID
    public Dictionary<Vector2Int, int> flags = new Dictionary<Vector2Int, int>();  // loc and player ID
    public Dictionary<int, List<Tactic>> unusedTactics;
    public Dictionary<int, List<Tactic>> usedTactics;
    public Dictionary<int, List<Piece>> activePieces;
    public Dictionary<int, List<Piece>> inactivePieces;

    public string boardName;
    public int firstPlayer; //player ID
    public int secondPlayer;
    public Dictionary<int, Lineup> lineups;
    public Dictionary<int, int> ores;
    public Dictionary<int, int> actions;
    public int round = 1;
    public int time = 90;
    public int maxTime = 90;
    public int gameID;
    public bool gameStarts = false;
    public bool gameOver = false;
    public int victory = -1; // -1 if draw, otherwise playerID

    public GameInfo(Lineup playerLineup, int playerID, Lineup enemyLineup, int enemyID)
    {
        lineups = new Dictionary<int, Lineup>()
        {
            { playerID, playerLineup },
            { enemyID, enemyLineup }
        };
        SetOrder(playerID, enemyID);
        ores = new Dictionary<int, int>()
        {
            { playerID, 30 },
            { enemyID, 30 }
        };
        actions = new Dictionary<int, int>()
        {
            { playerID, 1 },
            { enemyID, 1 }
        };

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

    public void AddPiece(Piece piece, bool reactivate = false)
    {
        piece.active = true;
        board.Add(piece.GetCastle(), piece);
        if (piece.isAlly)
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
        Upload();
    }

    public void RemovePiece(Piece piece)
    {
        piece.active = false;
        board.Remove(piece.location);
        if (piece.isAlly)
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

    public int FindTactic(string tacticName, int playerID)
    {
        List<Tactic> tactics = unusedTactics[playerID];
        for (int i = 0; i < tactics.Count; i++)
            if (tactics[i].tacticName == tacticName)
                return i;
        return -1;
    }

    public void SetOrder(int player1, int player2)
    {
        if(Random.Range(1,2)%2 == 1)
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

    public void SetGameID(int value)
    {
        gameID = value;
        //firstPlayer.gameID = gameID
        //secondPlayer.gameID = gameID
        Upload();
    }

    public void Move(Vector2Int from, Vector2Int to)
    {
        Piece piece = board[from];
        board.Remove(from);
        board.Add(to, piece);
        Upload();
    }

    public void ChangeOre(int deltaAmount)
    {
        ores[Login.playerID] += deltaAmount;
        Upload();
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
        return JsonUtility.ToJson(gameInfo);
    }
    public static GameInfo JsonToClass(string json)
    {
        return JsonUtility.FromJson<GameInfo>(json);
    }
    public void Upload()
    {
        return;
        WWWForm infoToPhp = new WWWForm(); //create WWWform to send to php script
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));
        infoToPhp.AddField("userJson", ClassToJson(this));

        WWW sendToPhp = new WWW("http://localhost:8888/update_userinfo.php", infoToPhp);
        while (!sendToPhp.isDone) { }
    }
    public static GameInfo Download()
    {
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("email", PlayerPrefs.GetString("email"));

        WWW sendToPhp = new WWW("http://localhost:8888/download_userinfo.php", infoToPhp);

        while (!sendToPhp.isDone) { }
        return JsonToClass(sendToPhp.text);  //sendToPhp.text is the userInfo json file
    }
}