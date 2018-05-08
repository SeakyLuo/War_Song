using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static OnEnterGame onEnterGame;
    public static BoardSetup boardSetup;
    public static Dictionary<string, List<Vector2Int>> castles;

    // Use this for initialization
    void Start () {
        onEnterGame = GameObject.Find("UIPanel").GetComponent<OnEnterGame>();
        boardSetup = onEnterGame.boardSetup;
        castles = new Dictionary<string, List<Vector2Int>>()
        {
            {"Advisor", boardSetup.boardAttributes.AdvisorCastle() },
            {"Elephant", boardSetup.boardAttributes.ElephantCastle()  },
            {"Horse", boardSetup.boardAttributes.HorseCastle()  },
            {"Chariot", boardSetup.boardAttributes.ChariotCastle()  },
            {"Cannon", boardSetup.boardAttributes.CannonCastle()  },
            {"Soldier", boardSetup.boardAttributes.SoldierCastle()  },
        };
    }

    public static void AddPiece(Collection collection, Vector2Int castle, bool isAlly)
    {
        boardSetup.AddPiece(collection, castle, isAlly);
    }

    public static void Eliminate(Piece piece)
    {
        Destroy(boardSetup.pieces[piece.location]);
        boardSetup.pieces.Remove(piece.location);
        GameInfo.Remove(piece);
    }

    public static void Eliminate(Vector2Int loc)
    {
        Destroy(boardSetup.pieces[loc]);
        boardSetup.pieces.Remove(loc);
        GameInfo.Remove(GameInfo.board[loc]);
    }

    public static void TriggerTrap(Vector2Int loc)
    {
        if(GameInfo.traps.ContainsKey(loc))
        {
            // trigger it
            InfoLoader.FindTrap(GameInfo.traps[loc]).trigger.Activate();
            GameInfo.traps.Remove(loc);
        }
    }

    public static void PlaceTrap(Vector2Int loc, string trapName)
    {
        GameInfo.traps.Add(loc, trapName);
    }
         
    public static void PlaceFlag(Vector2Int loc, bool isAlly)
    {
        
    }

    public static bool ChangeOre(int deltaAmount)
    {
        if(GameInfo.ores[InfoLoader.user.playerID] + deltaAmount < 0)
        {
            onEnterGame.NotEnoughOres();
            return false;
        }
        GameInfo.ores[InfoLoader.user.playerID] += deltaAmount;
        onEnterGame.SetOreText();
        return true;
    }

    public static bool ChangeCoin(int deltaAmount)
    {
        if(InfoLoader.user.coins + deltaAmount < 0)
        {
            onEnterGame.NotEnoughCoins();
            return false;
        }
        InfoLoader.user.coins += deltaAmount;
        onEnterGame.SetCoinText();
        return true;
    }

    public static List<Vector2Int> FindCastles(string type) { return castles[type]; }
}
