using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Database {
    public static List<string> types = new List<string> { "General", "Advisor", "Elephant", "Horse", "Chariot", "Cannon", "Soldier", "Tactic" };
    public static Dictionary<string, PieceAttributes> standardAttributes = new Dictionary<string, PieceAttributes>();
    public static Dictionary<string, PieceAttributes> pieces = new Dictionary<string, PieceAttributes>();
    public static Dictionary<string, TacticAttributes> tactics = new Dictionary<string, TacticAttributes>();
    public static Dictionary<string, BoardAttributes> boards = new Dictionary<string, BoardAttributes>();
    public static Dictionary<string, TrapAttributes> traps = new Dictionary<string, TrapAttributes>();
    public static Dictionary<string, ContractAttributes> contracts = new Dictionary<string, ContractAttributes>();
    public static Dictionary<string, MissionAttributes> missions = new Dictionary<string, MissionAttributes>();

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
        {"Board", new List<string>() }, {"Trap", new List<string>() }, {"Contract", new List<string>() }, { "Mission", new List<string>() }
    };

    private static List<string> loadType = new List<string> { "Standard Piece", "Piece", "Tactic", "Board", "Trap", "Contract", "Mission" };
    private static List<string> randomType = new List<string> { "Standard Piece", "Piece", "Tactic", "Trap" };

    public Database()
    {
        
    }

    public void Init()
    {
        if (standardAttributes.Count != 0) return;
        FindAttributes();
    }

    private void FindAttributes()
    {
        //WriteDirectories();
        ReadDirectories();
        foreach (string type in loadType)
        {
            foreach (string folder in directories[type])
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
                else if (type == "Mission")
                {
                    MissionAttributes attributes = LoadMissionAttributes(folder);
                    missions.Add(attributes.Name, attributes);
                    missionList.Add(attributes.Name);
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
        foreach(string path in textAsset.text.Split('\n'))
        {
            if (path == "") continue;
            int index = path.IndexOf('/');
            directories[path.Substring(0, index)].Add(path.Substring(index + 1).Trim());
        }
    }

    public static Sprite RandomImage()
    {
        string type = randomType[Random.Range(0, randomType.Count)];
        List<string> directory = directories[type];
        string name = directory[Random.Range(0, directory.Count)];
        Sprite image = null;
        if (type.EndsWith("Piece")) image = FindPieceAttributes(name).image;
        else if (type == "Tactic") image = FindTacticAttributes(name).image;
        else if (type == "Trap") image = FindTrapAttributes(name).image;
        return image;
    }
    public static string RandomTrap() { return trapList[Random.Range(0, trapList.Count)]; }
    public static string RandomMission() { return missionList[Random.Range(0, missionList.Count)]; }

    public static string FindType(string name)
    {
        foreach (string type in loadType)
            if (directories[type].Contains(name))
                return type;
        return "";
    }

    public static BoardAttributes FindBoardAttributes(string name) { return boards[name]; }
    public static PieceAttributes FindPieceAttributes(string name)
    {
        if (name.StartsWith("Standard ")) return standardAttributes[name];
        return pieces[name];
    }
    public static TacticAttributes FindTacticAttributes(string name) { return tactics[name]; }
    public static TrapAttributes FindTrapAttributes(string name) { return traps[name]; }
    public static ContractAttributes FindContractAttributes(string name) { return contracts[name]; }
    public static MissionAttributes FindMissionAttributes(string name) { return missions[name]; }

    public static BoardAttributes LoadBoardAttributes(string name) { return Resources.Load<BoardAttributes>("Board/" + name + "/Attributes"); }
    public static PieceAttributes LoadPieceAttributes(string name)
    {
        if (name.StartsWith("Standard ")) return Resources.Load<PieceAttributes>("Standard Piece/" + name + "/Attributes");
        return Resources.Load<PieceAttributes>("Piece/" + name + "/Attributes");
    }
    public static TacticAttributes LoadTacticAttributes(string name) { return Resources.Load<TacticAttributes>("Tactic/" + name + "/Attributes"); }
    public static TrapAttributes LoadTrapAttributes(string name) { return Resources.Load<TrapAttributes>("Trap/" + name + "/Attributes"); }
    public static ContractAttributes LoadContractAttributes(string name) { return Resources.Load<ContractAttributes>("Contract/" + name + "/Attributes"); }
    public static MissionAttributes LoadMissionAttributes(string name) { return Resources.Load<MissionAttributes>("Mission/" + name + "/Attributes"); }
    public static Trigger LoadPieceTrigger(string name)
    {
        if (name.StartsWith("Standard ")) return Resources.Load<Trigger>("Standard Piece/" + name + "/Trigger");
        return Resources.Load<Trigger>("Piece/" + name + "/Trigger");
    }
    public static TacticTrigger LoadTacticTrigger(string name)
    {
        if (FindType(name) == "Tactic") return Resources.Load<TacticTrigger>("Tactic/" + name + "/Trigger");
        return Resources.Load<TacticTrigger>("Trap/" + name + "/Trigger");
    }

    public static void WriteFile(string content, string filename = "WriteFile")
    {
        StreamWriter file = new StreamWriter(@"C:\Users\Seaky\Desktop\" + filename + ".txt");
        file.WriteLine(content);
        file.Close();
    }
}