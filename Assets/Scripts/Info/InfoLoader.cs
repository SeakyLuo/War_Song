using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class InfoLoader : MonoBehaviour {

    public static UserInfo user;
    public static List<BoardAttributes> boards = new List<BoardAttributes>();
    public static string switchSceneCaller = "Main";
    public static Dictionary<string, PieceAttributes> standardAttributes = new Dictionary<string, PieceAttributes>();
    public PieceAttributes standardGeneral, standardAdvisor, standardElephant, standardHorse, standardChariot, standardCannon, standardSoldier;

    private void Awake()
    {
        user = new CheatAccount();
        standardAttributes = new Dictionary<string, PieceAttributes>(){
            { "Standard General", standardGeneral },
            { "Standard Advisor", standardAdvisor },
            { "Standard Elephant", standardElephant },
            { "Standard Horse", standardHorse },
            { "Standard Chariot", standardChariot },
            { "Standard Cannon", standardCannon },
            { "Standard Soldier", standardSoldier }
        };
    }

    // Use this for initialization
    void Start () {
        boards.Add(FindBoardAttributes("Standard Board"));
        boards.Add(FindBoardAttributes("River Board"));
        //boards = Sorted(boards);
    }

    // Needs Sort By Last Use
    private List<BoardAttributes> Sorted(List<BoardAttributes> boardAttributes)
    {
        List<BoardAttributes> newList = boardAttributes;
        BoardAttributes standardBoard = boardAttributes[0];
        foreach (BoardAttributes attribute in boardAttributes)
        {
            if (attribute.boardName == "Standard Board")
            {
                standardBoard = attribute;
                newList.Remove(attribute);
                break;
            }
        }
        newList.OrderBy(BoardAttributes => BoardAttributes.boardName);
        newList.Insert(0, standardBoard);
        return newList;
    }

    public static string Vec2ToString(Vector2Int vec) { return vec.x.ToString() + vec.y.ToString(); }
    public static Vector2Int StringToVec2(string loc) { return new Vector2Int((int)Char.GetNumericValue(loc[0]), (int)Char.GetNumericValue(loc[1])); }
    public static BoardAttributes FindBoardAttributes(string boardName) { return Resources.Load<BoardAttributes>("Board/" + boardName + "/Attributes"); }
    public static PieceAttributes FindPieceAttributes(string pieceName)
    {
        if (pieceName.StartsWith("Standard ")) return InfoLoader.standardAttributes[pieceName];
        return Resources.Load<PieceAttributes>("Piece/" + pieceName + "/Attributes");
    }
    public static TacticAttributes FindTacticAttributes(string tacticName) { return Resources.Load<TacticAttributes>("Tactic/" + tacticName + "/Attributes"); }
    public static Trap FindTrap(string trapName) { return Resources.Load<Trap>("Trap/" + trapName + "/Trap"); }
}