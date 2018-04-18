using UnityEngine;
using System.Collections.Generic;

public class Collection
{
    public string name = "";
    public string type = "";
    public int count = 1;
    public int health = 0;

    public static string[] types = new string[] { "General", "Advisor", "Elephant", "Horse", "Chariot", "Cannon", "Soldier", "Tactic" };
    public static Collection General = standardCollection("General");
    public static Collection Advisor = standardCollection("Advisor");
    public static Collection Elephant = standardCollection("Elephant");
    public static Collection Horse = standardCollection("Horse");
    public static Collection Chariot = standardCollection("Chariot");
    public static Collection Cannon = standardCollection("Cannon");
    public static Collection Soldier = standardCollection("Soldier");
    public static Dictionary<string, Collection> standardCollectionDict = new Dictionary<string, Collection>
    {
        {"General", General },
        {"Advisor", Advisor },
        {"Elephant", Elephant },
        {"Horse", Horse },
        {"Chariot", Chariot },
        {"Cannon", Cannon },
        {"Soldier", Soldier }
    };

    public Collection() { }

    public Collection(PieceAttributes attributes, int Count = 1, int Health = 0)
    {
        name = attributes.Name;
        type = attributes.type;
        count = Count;
        health = Health;
        if (health == 0) health = attributes.health;
    }

    public Collection(string tacticName, int Count = 1)
    {
        name = tacticName;
        type = "Tactic";
        count = Count;
        health = Resources.Load<TacticAttributes>("Tactics/Info/" + tacticName + "/Attributes").goldCost;
    }

    public Collection(TacticAttributes attributes, int Count = 1)
    {
        name = attributes.Name;
        type = "Tactic";
        count = Count;
        health = attributes.goldCost;
    }

    public Collection(string Name, string Type, int Count = 1, int Health = 0)
    {
        name = Name;
        type = Type;
        count = Count;
        health = Health;
        if (Type == "Tactic") health = Resources.Load<TacticAttributes>("Tactics/Info/" + Name + "/Attributes").goldCost;
        else if (Health == 0 && !Name.StartsWith("Standard ")) health = Resources.Load<PieceAttributes>("Pieces/Info/" + Name + "/Attributes").health;
    }

    public static Collection standardCollection(string type)
    {
        return new Collection("Standard " + type, type);
    }

    public bool IsEmpty()
    {
        return name == "" && type == "" && count == 1 && health == 0;
    }
}