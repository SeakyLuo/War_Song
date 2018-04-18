using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo
{
    public static Dictionary<Vector2Int, Piece> board = new Dictionary<Vector2Int, Piece>();
    public static List<string> tactics = new List<string>();
    public static List<string> usedTactics = new List<string>();
    public static List<Piece> activeAlly = new List<Piece>(),
                           inactiveAlly = new List<Piece>(),
                           activeEnemy = new List<Piece>(),
                           inactiveEnemy = new List<Piece>();

    public static void Clear()
    {
        board.Clear();
        tactics.Clear();
        usedTactics.Clear();
        activeAlly.Clear();
        inactiveAlly.Clear();
        activeEnemy.Clear();
        inactiveEnemy.Clear();
    }
}