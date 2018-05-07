using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo
{
    public static Dictionary<Vector2Int, List<Piece>> castles = new Dictionary<Vector2Int, List<Piece>>();
    public static Dictionary<Vector2Int, Piece> board = new Dictionary<Vector2Int, Piece>();
    public static Dictionary<Vector2Int, string> traps = new Dictionary<Vector2Int, string>();
    public static List<string> tactics = new List<string>();
    public static List<string> usedTactics = new List<string>();
    public static List<Piece> activeAllies = new List<Piece>(),
                           inactiveAllies = new List<Piece>(),
                           activeEnemies = new List<Piece>(),
                           inactiveEnemies = new List<Piece>();

    public static bool pieceMoved = false;
    public static bool tacticUsed = false;
    public static bool abilityActivated = false;
    public static bool actionTaken = false;

    public static int firstPlayer; //player ID
    public static int secondPlayer;
    public static Dictionary<int, int> ores;
    public static int round = 1;
    public static int time = 90;
    public static int maxTime = 90;
    public static int gameID;

    //public GameInfo()
    //{

    //}

    public static void Add(Piece piece, bool reactivate = false)
    {
        piece.active = true;
        board.Add(piece.GetCastle(), piece);
        if (piece.IsAlly())
        {
            activeAllies.Add(piece);
            if (reactivate) inactiveAllies.Remove(piece);
        }
        else
        {
            activeEnemies.Add(piece);
            if (reactivate) inactiveEnemies.Remove(piece);
        }
    }

    public static void Remove(Piece piece)
    {
        piece.active = false;
        board.Remove(piece.location);
        if (piece.IsAlly())
        {
            activeAllies.Remove(piece);
            inactiveAllies.Add(piece);
        }
        else
        {
            activeEnemies.Remove(piece);
            inactiveEnemies.Add(piece);
        }
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
        ores = new Dictionary<int, int>()
        {
            { firstPlayer, 30 },
            { secondPlayer, 30 }
        };
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
    }

    public static void Clear()
    {
        board.Clear();
        tactics.Clear();
        usedTactics.Clear();
        activeAllies.Clear();
        inactiveAllies.Clear();
        activeEnemies.Clear();
        inactiveEnemies.Clear();
        round = 1;
    }

    public static void ClassToJson()
    {

    }

    public static void JsonToClass()
    {

    }
}