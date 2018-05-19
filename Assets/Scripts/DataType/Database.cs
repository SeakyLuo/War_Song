using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

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
    public static List<string> contractList = new List<string>();
    public static List<string> missionList = new List<string>();
    public static Dictionary<string, List<string>> directories = new Dictionary<string, List<string>>{
        {"Standard Piece", new List<string>() }, {"Piece", new List<string>() }, {"Tactic", new List<string>() },
        {"Board", new List<string>() }, {"Trap", new List<string>() }, {"Contract", new List<string>() }
    };

    private static List<string> loadType = new List<string> { "Standard Piece", "Piece", "Tactic", "Board", "Trap", "Contract" };

    public Database()
    {
        
    }

    public void Init()
    {
        if (standardAttributes.Count != 0) return;
        FindAttributes();
    }

    private void FindAttributes(string type, StreamWriter writer)
    {
        string path = Application.dataPath + "/Resources/" + type + "/";
        int start = path.Length;
        writer.WriteLine(Directory.Exists(path));
        foreach (string file in directories[type])
        {
            string folder = file.Substring(start);
            writer.WriteLine(folder);
            Debug.Log(folder);
            if (type == "Standard Piece")
            {
                writer.WriteLine(1.25);
                PieceAttributes attributes = LoadPieceAttributes(folder); if (attributes == null) writer.WriteLine(1); writer.WriteLine(1.5);
                standardAttributes.Add(attributes.Name, attributes);
                writer.WriteLine(1.75);
            }
            else if (type == "Piece")
            {
                writer.WriteLine(2.25);
                PieceAttributes attributes = LoadPieceAttributes(folder); if (attributes == null) writer.WriteLine(2); writer.WriteLine(2.5);
                pieces.Add(attributes.Name, attributes);
                pieceList.Add(attributes.Name);
                pieceListDict[attributes.type].Add(attributes.Name); writer.WriteLine(2.75);
            }
            else if (type == "Tactic")
            {
                writer.WriteLine(3.25);
                TacticAttributes attributes = LoadTacticAttributes(folder); if (attributes == null) writer.WriteLine(3); writer.WriteLine(3.5);
                tactics.Add(attributes.Name, attributes);
                tacticList.Add(attributes.Name); writer.WriteLine(3.75);
            }
            else if (type == "Board")
            {
                writer.WriteLine(4.25);
                BoardAttributes attributes = LoadBoardAttributes(folder); if (attributes == null) writer.WriteLine(4); writer.WriteLine(4.5);
                boards.Add(attributes.Name, attributes);
                boardList.Add(attributes.Name); writer.WriteLine(4.75);
            }
            else if (type == "Trap")
            {
                writer.WriteLine(5.25);
                TrapAttributes attributes = LoadTrapAttributes(folder); if (attributes == null) writer.WriteLine(5); writer.WriteLine(5.5);
                traps.Add(attributes.Name, attributes);
                trapList.Add(attributes.Name); writer.WriteLine(5.75);
            }
            else if (type == "Contract")
            {
                writer.WriteLine(6.25);
                ContractAttributes attributes = LoadContractAttributes(folder); if (attributes == null) writer.WriteLine(6); writer.WriteLine(6.5);
                contracts.Add(attributes.Name, attributes);
                contractList.Add(attributes.Name); writer.WriteLine(6.75);
            }
        }
        writer.WriteLine(0);
    }

    private void FindAttributes()
    {
        //WriteDirectories();
        ReadDirectories();
        foreach (string type in loadType)
        {
            foreach( string folder in directories[type])
            {
                if (type == "Standard Piece")
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
                    contractList.Add(attributes.Name);
                }
            }
        }
    }

    private static void WriteDirectories()
    {
        using (StreamWriter file = new StreamWriter(Application.dataPath + "/Resources/Directories.txt", false))
        {
            foreach (string type in loadType)
            {
                string path = Application.dataPath + "/Resources/" + type + "/";
                int startIndex = (Application.dataPath + "/Resources/").Length;
                foreach (string directory in Directory.GetDirectories(path))
                {
                    file.WriteLine(directory.Substring(startIndex));
                }
            }
        }
    }
    private static void ReadDirectories()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Directories");
        string text = textAsset.text;
        var list = text.Split('\n');
        foreach(string path in list)
        {
            if (path == "") continue;
            int index = path.IndexOf('/');
            directories[path.Substring(0, index)].Add(path.Substring(index + 1).Trim());
        }
    }

    public static string RandomTrap() { return trapList[Random.Range(0, trapList.Count)]; }

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

    public static string Vec2ToString(Vector2Int vec) { return vec.x.ToString() + vec.y.ToString(); }
    public static Vector2Int StringToVec2(string loc) { return new Vector2Int((int)System.Char.GetNumericValue(loc[0]), (int)System.Char.GetNumericValue(loc[1])); }
}