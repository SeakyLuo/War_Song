using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Database {
    public static List<string> types = new List<string> { "General", "Advisor", "Elephant", "Horse", "Chariot", "Cannon", "Soldier", "Tactic" };
    public static Dictionary<string, PieceAttributes> standardAttributes = new Dictionary<string, PieceAttributes>();
    public static Dictionary<string, PieceAttributes> pieces = new Dictionary<string, PieceAttributes>();
    public static Dictionary<string, TacticAttributes> tactics = new Dictionary<string, TacticAttributes>();
    public static Dictionary<string, BoardAttributes> boards = new Dictionary<string, BoardAttributes>();
    public static Dictionary<string, TrapAttributes> traps = new Dictionary<string, TrapAttributes>();
    public static Dictionary<string, ContractAttributes> contracts = new Dictionary<string, ContractAttributes>();

    public static List<string> pieceList = new List<string>();
    public static List<string> tacticList = new List<string>();
    public static Dictionary<string, List<string>> pieceListDict = new Dictionary<string, List<string>>
    {
        {"General", new List<string>() }, {"Advisor", new List<string>() }, {"Elephant", new List<string>() },
        {"Horse", new List<string>() }, {"Chariot", new List<string>() }, {"Cannon", new List<string>() }, {"Soldier", new List<string>() }
    };
    public static List<string> boardList = new List<string>();
    public static List<string> trapList = new List<string>();

    public static List<string> missionList = new List<string>();

    public Database()
    {
        FindAttributes("Standard Piece");
        FindAttributes("Piece");
        FindAttributes("Tactic");
        FindAttributes("Trap");
        FindAttributes("Contract");;
    }

    private void FindAttributes(string type)
    {
        string path = "Assets/Resources/" + type + "/";
        foreach(string file in Directory.GetFiles(path))
        {
            string folder = file.Substring(path.Length);
            folder = folder.Substring(0, folder.Length - 5);
            if(type == "Standard Piece")
            {
                PieceAttributes attributes = LoadPieceAttributes(folder);
                standardAttributes.Add(attributes.Name, attributes);
            }
            else if (type == "Piece")
            {
                PieceAttributes attributes = LoadPieceAttributes(folder);
                pieces.Add(attributes.Name, attributes);
                pieceList.Add(attributes.Name);
                pieceListDict[attributes.type].Add(attributes.Name);
            }
            else if (type == "Tactic")
            {
                TacticAttributes attributes = LoadTacticAttributes(folder);
                tactics.Add(attributes.Name, attributes);
                tacticList.Add(attributes.Name);
            }
            else if (type == "Board")
            {
                BoardAttributes attributes = LoadBoardAttributes(folder);
                boards.Add(attributes.Name, attributes);
                boardList.Add(attributes.Name);
            }
            else if (type == "Trap")
            {
                TrapAttributes attributes = LoadTrapAttributes(folder);
                traps.Add(attributes.Name, attributes);
                trapList.Add(attributes.Name);
            }
            else if (type == "Contract")
            {
                ContractAttributes attributes = LoadContractAttributes(folder);
                contracts.Add(attributes.Name, attributes);
            }
        }
    }

    public static BoardAttributes FindBoardAttributes(string boardName) { return boards[boardName]; }
    public static PieceAttributes FindPieceAttributes(string pieceName)
    {
        if (pieceName.StartsWith("Standard ")) return standardAttributes[pieceName];
        return pieces[pieceName];
    }
    public static TacticAttributes FindTacticAttributes(string tacticName) { return tactics[tacticName]; }
    public static TrapAttributes FindTrapAttributes(string trapName) { return traps[trapName]; }
    public static ContractAttributes FindContractAttributes(string contractName) { return contracts[contractName]; }

    public static BoardAttributes LoadBoardAttributes(string boardName) { return Resources.Load<BoardAttributes>("Board/" + boardName + "/Attributes"); }
    public static PieceAttributes LoadPieceAttributes(string pieceName)
    {
        if (pieceName.StartsWith("Standard ")) return Resources.Load<PieceAttributes>("Standard Piece/" + pieceName + "/Attributes");
        return Resources.Load<PieceAttributes>("Piece/" + pieceName + "/Attributes");
    }
    public static TacticAttributes LoadTacticAttributes(string tacticName) { return Resources.Load<TacticAttributes>("Tactic/" + tacticName + "/Attributes"); }
    public static TrapAttributes LoadTrapAttributes(string trapName) { return Resources.Load<TrapAttributes>("Trap/" + trapName + "/Attributes"); }
    public static ContractAttributes LoadContractAttributes(string contractName) { return Resources.Load<ContractAttributes>("Contract/" + contractName + "/Attributes"); }
}