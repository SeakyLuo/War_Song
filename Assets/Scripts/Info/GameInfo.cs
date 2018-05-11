using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo
{
    public static Dictionary<Vector2Int, List<Piece>> castles = new Dictionary<Vector2Int, List<Piece>>();
    public static Dictionary<Vector2Int, Piece> board = new Dictionary<Vector2Int, Piece>();
    public static Dictionary<Vector2Int, KeyValuePair<string, int>> traps = new Dictionary<Vector2Int, KeyValuePair<string, int>>(); // loc and trap name & creator ID
    public static Dictionary<Vector2Int, int> flags = new Dictionary<Vector2Int, int>();  // loc and player ID
    public static List<string> unusedTactics = new List<string>();
    public static List<string> usedTactics = new List<string>();
    public static List<Piece> activeAllies = new List<Piece>(),
                           inactiveAllies = new List<Piece>(),
                           activeEnemies = new List<Piece>(),
                           inactiveEnemies = new List<Piece>();

    public static string boardName;
    public static int firstPlayer; //player ID
    public static int secondPlayer;
    public static Dictionary<int, int> ores;
    public static Dictionary<int, int> actions;
    public static int round = 1;
    public static int time = 90;
    public static int maxTime = 90;
    public static int gameID;
    public static bool gameStarts = false;
    public static bool gameOver = false;

    public GameInfo()
    {
        SetOrder(InfoLoader.user.playerID, 100000000);
        ores = new Dictionary<int, int>()
        {
            { firstPlayer, 30 },
            { secondPlayer, 30 }
        };
        actions = new Dictionary<int, int>();
        actions.Add(firstPlayer, 1);
        actions.Add(secondPlayer, 1);
        gameStarts = true;
        
        SetGameID(1);
        Lineup lineup = InfoLoader.user.lineups[InfoLoader.user.lastLineupSelected];
        boardName = lineup.boardName;
        unusedTactics = new List<string>(lineup.tactics);
    }
    public static int TheOtherPlayerID()
    {
        if (firstPlayer == InfoLoader.user.playerID) return secondPlayer;
        else return firstPlayer;
    }

    public static void Add(Piece piece, bool reactivate = false)
    {
        piece.active = true;
        board.Add(piece.GetCastle(), piece);
        if (piece.isAlly)
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
        if (piece.isAlly)
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

    public static bool IsAllyAlive(Collection collection)
    {
        foreach(Piece piece in activeAllies)
            if (piece.SameCollection(collection))
                return true;
        return false;
    }

    public static void AddTactic(string tacticName)
    {
        unusedTactics.Add(tacticName);
    }

    public static void RemoveTactic(string tacticName)
    {
        unusedTactics.Remove(tacticName);
        usedTactics.Add(tacticName);
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
    }

    public static void Clear()
    {
        board.Clear();
        unusedTactics.Clear();
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

    public static void JsonToClass(GameInfo gameInfo)
    {

    }
}