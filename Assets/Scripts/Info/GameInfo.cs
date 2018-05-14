using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo
{
    public static Dictionary<Vector2Int, List<Piece>> castles = new Dictionary<Vector2Int, List<Piece>>();
    public static Dictionary<Vector2Int, Piece> board = new Dictionary<Vector2Int, Piece>();
    public static Dictionary<Vector2Int, KeyValuePair<string, int>> traps = new Dictionary<Vector2Int, KeyValuePair<string, int>>(); // loc and trap name & creator ID
    public static Dictionary<Vector2Int, int> flags = new Dictionary<Vector2Int, int>();  // loc and player ID
    public static Dictionary<int, List<Tactic>> unusedTactics;
    public static Dictionary<int, List<Tactic>> usedTactics;
    public static Dictionary<int, List<Piece>> activePieces;
    public static Dictionary<int, List<Piece>> inactivePieces;

    public static string boardName;
    public static int firstPlayer; //player ID
    public static int secondPlayer;
    public static Dictionary<int, Lineup> lineups;
    public static Dictionary<int, int> ores;
    public static Dictionary<int, int> actions;
    public static int round = 1;
    public static int time = 90;
    public static int maxTime = 90;
    public static int gameID;
    public static bool gameStarts = false;
    public static bool gameOver = false;
    public static int victory = -1; // -1 if draw, otherwise playerID

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
    public static int TheOtherPlayer()
    {
        if (firstPlayer == InfoLoader.playerID) return secondPlayer;
        else return firstPlayer;
    }

    public static void AddPiece(Piece piece, bool reactivate = false)
    {
        piece.active = true;
        board.Add(piece.GetCastle(), piece);
        if (piece.isAlly)
        {
            activePieces[InfoLoader.playerID].Add(piece);
            if (reactivate) inactivePieces[InfoLoader.playerID].Remove(piece);
        }
        else
        {
            activePieces[InfoLoader.playerID].Add(piece);
            if (reactivate) inactivePieces[InfoLoader.playerID].Remove(piece);
        }
        if (castles.ContainsKey(piece.GetCastle())) castles[piece.GetCastle()].Add(piece);
        else castles.Add(piece.GetCastle(), new List<Piece> { piece });
        // upload
    }

    public static void RemovePiece(Piece piece)
    {
        piece.active = false;
        board.Remove(piece.location);
        if (piece.isAlly)
        {
            activePieces[InfoLoader.playerID].Remove(piece);
            inactivePieces[InfoLoader.playerID].Add(piece);
        }
        else
        {
            activePieces[TheOtherPlayer()].Remove(piece);
            inactivePieces[TheOtherPlayer()].Add(piece);
        }
        castles[piece.location].Remove(piece);
        // upload
    }

    public static void AddTactic(Tactic tactic)
    {
        int index = 0;
        List<Tactic> tactics = unusedTactics[InfoLoader.playerID];
        if (tactics.Count == 0 || tactic < tactics[0]) index = 0;
        else if (tactic > tactics[tactics.Count - 1]) index = tactics.Count;
        else
        {
            for (int i = 0; i < unusedTactics[InfoLoader.playerID].Count - 1; i++)
            {
                if (tactic > tactics[i] && tactic < tactics[i+1])
                {
                    index = i + 1;
                    break;
                }
            }
        }
        unusedTactics[InfoLoader.playerID].Insert(index, tactic);
        // upload
    }

    public static void RemoveTactic(int index)
    {
        usedTactics[InfoLoader.playerID].Add(unusedTactics[InfoLoader.playerID][index]);
        unusedTactics[InfoLoader.playerID].RemoveAt(index);
        // upload
    }

    public static int FindTactic(string tacticName, int playerID)
    {
        List<Tactic> tactics = unusedTactics[playerID];
        for (int i = 0; i < tactics.Count; i++)
            if (tactics[i].tacticName == tacticName)
                return i;
        return -1;
    }

    public static void SetOrder(int player1, int player2)
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

    }

    public static void SetGameID(int value)
    {
        gameID = value;
        //firstPlayer.gameID = gameID
        //secondPlayer.gameID = gameID
    }

    public static void Move(Vector2Int from, Vector2Int to)
    {
        Piece piece = board[from];
        board.Remove(from);
        board.Add(to, piece);
        // upload
    }

    public static void Clear()
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
}