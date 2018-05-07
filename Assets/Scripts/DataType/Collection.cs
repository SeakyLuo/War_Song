using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Collection
{
    public string name = "";
    public string type = "";
    public int count = 1;
    public int health = 0;

    private int oreCost = 0; //tmp

    public static List<string> types = new List<string> { "General", "Advisor", "Elephant", "Horse", "Chariot", "Cannon", "Soldier", "Tactic" };
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
        if (Health == 0) health = attributes.health;
        oreCost = attributes.oreCost;
    }

    public Collection(string tacticName, int Count = 1)
    {
        name = tacticName;
        type = "Tactic";
        count = Count;
        TacticAttributes attributes = Resources.Load<TacticAttributes>("Tactics/" + tacticName + "/Attributes");
        health = attributes.goldCost;
        oreCost = attributes.oreCost;
    }

    public Collection(TacticAttributes attributes, int Count = 1)
    {
        name = attributes.Name;
        type = "Tactic";
        count = Count;
        health = attributes.goldCost;
        oreCost = attributes.oreCost;
    }

    public Collection(string Name, string Type, int Count = 1, int Health = 0)
    {
        name = Name;
        type = Type;
        count = Count;
        health = Health;
        if (Type == "Tactic")
        {
            TacticAttributes attributes = Resources.Load<TacticAttributes>("Tactics/" + Name + "/Attributes");
            health = attributes.goldCost;
            oreCost = attributes.oreCost;
        }
        else if (!Name.StartsWith("Standard "))
        {
            PieceAttributes attributes = Resources.Load<PieceAttributes>("Pieces/" + Name + "/Attributes");
            if (Health == 0) health = attributes.health;
            oreCost = attributes.oreCost;
        }
    }

    public static Collection standardCollection(string type)
    {
        return new Collection("Standard " + type, type);
    }

    public bool IsEmpty()
    {
        return name == "" && type == "" && count == 1 && health == 0;
    }

    public bool LessThan(Collection collection)
    {
        if(type == "Tactic" && collection.type == type)
        {
            return (oreCost < collection.oreCost) ||
                   (oreCost == collection.oreCost && health < collection.health) ||
                   (oreCost == collection.oreCost && health == collection.health && name.CompareTo(collection.name) < 0);
        }
        else
        {
            int typeIndex = types.IndexOf(type), collectionTypeIndex = types.IndexOf(collection.type);
            return (typeIndex < collectionTypeIndex) ||
                   (typeIndex == collectionTypeIndex && oreCost < collection.oreCost) ||
                   (typeIndex == collectionTypeIndex && oreCost == collection.oreCost && name.CompareTo(collection.name) < 0) ||
                   (typeIndex == collectionTypeIndex && name == collection.name && health < collection.health);
        }
    }

    public bool GreaterThan(Collection collection)
    {
        return !LessThan(collection) && !Equals(collection);
    }

    public bool Equals(Collection collection)
    {
        return type == collection.type && oreCost == collection.oreCost && name == collection.name && health == collection.health;
    }
}