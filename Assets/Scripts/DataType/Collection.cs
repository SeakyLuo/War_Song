using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Collection
{
    public string name = "";
    public string type = "";
    public int count = 1;
    public int health = 0;
    public int oreCost = 0; //tmp

    public static Collection General = StandardCollection("General");
    public static Collection Advisor = StandardCollection("Advisor");
    public static Collection Elephant = StandardCollection("Elephant");
    public static Collection Horse = StandardCollection("Horse");
    public static Collection Chariot = StandardCollection("Chariot");
    public static Collection Cannon = StandardCollection("Cannon");
    public static Collection Soldier = StandardCollection("Soldier");
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

    public Collection(CardInfo cardInfo)
    {
        name = cardInfo.GetCardName();
        type = cardInfo.GetCardType();
        count = 1;
        oreCost = cardInfo.GetOreCost();
        health = cardInfo.GetHealth();
    }

    public Collection(PieceAttributes attributes, int Count = 1, int Health = 0)
    {
        name = attributes.Name;
        type = attributes.type;
        count = Count;
        oreCost = attributes.oreCost;
        health = Health;
        if (Health == 0) health = attributes.health;
    }

    public Collection(string tacticName, int Count = 1)
    {
        name = tacticName;
        type = "Tactic";
        count = Count;
        TacticAttributes attributes = Database.FindTacticAttributes(tacticName);
        oreCost = attributes.oreCost;
        health = attributes.goldCost;
    }

    public Collection(Tactic tactic, int Count = 1)
    {
        name = tactic.tacticName;
        type = "Tactic";
        count = Count;
        oreCost = tactic.oreCost;
        health = tactic.goldCost;
    }

    public Collection(TacticAttributes attributes, int Count = 1)
    {
        name = attributes.Name;
        type = "Tactic";
        count = Count;
        oreCost = attributes.oreCost;
        health = attributes.goldCost;
    }

    public Collection(string Name, string Type, int Count = 1, int Health = 0)
    {
        name = Name;
        type = Type;
        count = Count;
        health = Health;
        if (Type == "Tactic")
        {
            TacticAttributes attributes = Database.FindTacticAttributes(Name);
            health = attributes.goldCost;
            oreCost = attributes.oreCost;
        }
        else if (!Name.StartsWith("Standard "))
        {
            PieceAttributes attributes = Database.FindPieceAttributes(Name);
            if (Health == 0) health = attributes.health;
            oreCost = attributes.oreCost;
        }
    }

    public static Collection StandardCollection(string type)
    {
        return new Collection("Standard " + type, type);
    }


    public static void InsertCollection(List<Collection> collectionList, Collection insert)
    {
        int index = 0;
        if (collectionList.Count == 0 || insert < collectionList[0]) index = 0;
        else if (collectionList[collectionList.Count - 1] < insert) index = collectionList.Count;
        else
            for (int i = 0; i < collectionList.Count - 1; i++)
                if (insert.Equals(collectionList[i]))
                {
                    collectionList[i].count += insert.count;
                    return;
                }
                else if (collectionList[i] < insert && insert < collectionList[i + 1])
                {
                    index = i + 1;
                    break;
                }
        collectionList.Insert(index, insert);
    }

    public bool IsEmpty()
    {
        return name == "" && type == "" && count == 1 && health == 0;
    }

    public static bool operator < (Collection collection1, Collection collection2)
    {
        if(collection1.type == "Tactic" && collection2.type == collection1.type)
            return new Tactic(collection1) < new Tactic(collection2);
        else
        {
            int typeIndex = Database.types.IndexOf(collection1.type), typeIndex2 = Database.types.IndexOf(collection2.type);
            return (typeIndex < typeIndex2) ||
                   (typeIndex == typeIndex2 && collection1.oreCost < collection2.oreCost) ||
                   (typeIndex == typeIndex2 && collection1.oreCost == collection2.oreCost && collection1.name.CompareTo(collection2.name) < 0) ||
                   (typeIndex == typeIndex2 && collection1.name == collection2.name && collection1.health < collection2.health);
        }
    }

    public static bool operator > (Collection collection1, Collection collection2)
    {
        return !(collection1 < collection2) && !collection1.Equals(collection2);
    }

    public bool Equals(Collection collection)
    {
        if (collection.type == "Tactic") return name == collection.name;
        return name == collection.name && health == collection.health;
    }

    public bool IsStandard() { return name.StartsWith("Standard "); }
}