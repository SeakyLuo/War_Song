using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Database {

    public static List<PieceAttributes> pieces = new List<PieceAttributes>();
    public static List<TacticAttributes> tactics = new List<TacticAttributes>();
    public static Dictionary<string, List<PieceAttributes>> attributes = new Dictionary<string, List<PieceAttributes>>()
    {
        {"General", new List<PieceAttributes>() }, {"Advisor", new List<PieceAttributes>() }, {"Elephant", new List<PieceAttributes>() },
        {"Horse", new List<PieceAttributes>() }, {"Chariot", new List<PieceAttributes>() }, {"Cannon", new List<PieceAttributes>() }, {"Soldier", new List<PieceAttributes>() }
    };
    public static List<BoardAttributes> boards = new List<BoardAttributes>();
    public static List<Trap> traps = new List<Trap>();
    public static List<Mission> missions = new List<Mission>();

    public Database(Dictionary<string, List<Collection>> dict)
    {
        foreach(KeyValuePair<string,List<Collection>> pair in dict)
        {
            foreach(Collection collection in pair.Value)
            {
                if (collection.type == "Tactic") tactics.Add(Resources.Load<TacticAttributes>("Tactics/" + collection.name + "/Attributes"));
                else
                {
                    PieceAttributes pieceAttributes = Resources.Load<PieceAttributes>("Pieces/" + collection.name + "/Attributes");
                    pieces.Add(pieceAttributes);
                    attributes[collection.type].Add(pieceAttributes);
                }
            }
        }
    }

    public string FindDescription(string name)
    {
        foreach(PieceAttributes attributes in pieces)
            if (attributes.name == name)
                return attributes.description;
        return "";
    }

    public string FindDescription(Collection collection)
    {
        return FindDescription(collection.name);
    }
}