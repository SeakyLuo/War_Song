using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database {

    public static List<PieceAttributes> pieces;
    public static List<TacticAttributes> tactics;
    public static List<PieceAttributes> all, general, advisor, elephant, horse, chariot, cannon, soldier, tactic;
    public Database()
    {
        general = new List<PieceAttributes> { };
        advisor = new List<PieceAttributes> { };
        elephant = new List<PieceAttributes> { };
        horse = new List<PieceAttributes> { };
        chariot = new List<PieceAttributes> { };
        cannon = new List<PieceAttributes> { };
        soldier = new List<PieceAttributes> { };
        tactic = new List<PieceAttributes> { };
    }

    public Database(Dictionary<string, List<Collection>> dict)
    {
        foreach(KeyValuePair<string,List<Collection>> pair in dict)
        {
            foreach(Collection collection in pair.Value)
            {
                if (collection.type == "Tactic") tactics.Add(Resources.Load<TacticAttributes>("Tactics/Info/" + collection.name + "/Attributes"));
                else pieces.Add(Resources.Load<PieceAttributes>("Pieces/Info/" + collection.name + "/Attributes"));
            }
        }
    }

    public string FindDescription(string name)
    {
        foreach(PieceAttributes attributes in all)
            if (attributes.name == name)
                return attributes.description;
        return "";
    }

    public string FindDescription(Collection collection)
    {
        return FindDescription(collection.name);
    }
}