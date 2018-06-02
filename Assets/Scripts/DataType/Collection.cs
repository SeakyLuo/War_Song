using System.Collections.Generic;

[System.Serializable]
public class Collection
{
    public string name = "";
    public string type = "";
    public int count = 1;
    public int health = 0;

    public static Collection General = StandardCollection("General");
    public static Collection Advisor = StandardCollection("Advisor");
    public static Collection Elephant = StandardCollection("Elephant");
    public static Collection Horse = StandardCollection("Horse");
    public static Collection Chariot = StandardCollection("Chariot");
    public static Collection Cannon = StandardCollection("Cannon");
    public static Collection Soldier = StandardCollection("Soldier");

    public Collection() { }

    public Collection(CardInfo cardInfo)
    {
        name = cardInfo.GetCardName();
        type = cardInfo.GetCardType();
        count = 1;
        health = cardInfo.GetHealth();
    }

    public Collection(PieceAttributes attributes, int Count = 1, int Health = 0)
    {
        name = attributes.Name;
        type = attributes.type;
        count = Count;
        health = Health;
        if (Health == 0) health = attributes.health;
    }

    public Collection(string tacticName, int Count = 1)
    {
        name = tacticName;
        type = "Tactic";
        count = Count;
        TacticAttributes attributes = Database.FindTacticAttributes(tacticName);
        health = attributes.goldCost;
    }

    public Collection(Tactic tactic, int Count = 1)
    {
        name = tactic.tacticName;
        type = "Tactic";
        count = Count;
        health = tactic.goldCost;
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
        if (Type == "Tactic")
        {
            TacticAttributes attributes = Database.FindTacticAttributes(Name);
            health = attributes.goldCost;
        }
        else if (!Name.StartsWith("Standard "))
        {
            PieceAttributes attributes = Database.FindPieceAttributes(Name);
            if (Health == 0) health = attributes.health;
        }
    }

    public static Collection StandardCollection(string type)
    {
        return new Collection("Standard " + type, type);
    }

    public static void InsertCollection(List<Collection> collectionList, Collection insert)
    {
        if (collectionList.Count == 0 || insert < collectionList[0]) collectionList.Insert(0, insert);
        else if (collectionList[collectionList.Count - 1] < insert) collectionList.Add(insert);
        else
            for (int i = 0; i < collectionList.Count - 1; i++)
                if (insert.Equals(collectionList[i]))
                {
                    collectionList[i].count += insert.count;
                    return;
                }
                else if (collectionList[i] < insert && insert < collectionList[i + 1])
                {
                    collectionList.Insert(i + 1, insert);
                    return;
                }
    }

    public bool IsEmpty()
    {
        return name == "" && type == "" && count == 1 && health == 0;
    }

    public static bool operator < (Collection collection1, Collection collection2)
    {
        if(collection1.type == "Tactic" && collection2.type == "Tactic")
            return new Tactic(collection1) < new Tactic(collection2);
        else
        {
            int typeIndex1 = Database.types.IndexOf(collection1.type), typeIndex2 = Database.types.IndexOf(collection2.type);
            if (typeIndex1 != typeIndex2) return typeIndex1 < typeIndex2;
            int oreCost1 = Database.FindPieceAttributes(collection1.name).oreCost, oreCost2 = Database.FindPieceAttributes(collection2.name).oreCost;
            return oreCost1 < oreCost2 ||
                   (oreCost1 == oreCost2 && collection1.name.CompareTo(collection2.name) < 0) ||
                   (oreCost1 == oreCost2 && collection1.name == collection2.name && collection1.health < collection2.health);
        }
    }

    public static bool operator > (Collection collection1, Collection collection2)
    {
        return !(collection1 < collection2) && !collection1.Equals(collection2);
    }

    //public static bool operator == (Collection collection1, Collection collection2) { return collection1.Equals(collection2); }
    //public static bool operator != (Collection collection1, Collection collection2) { return !collection1.Equals(collection2); }

    public override bool Equals(object obj)
    {
        Collection collection = obj as Collection;
        if (collection.type == "Tactic" && type == "Tactic") return name == collection.name;
        return name == collection.name && health == collection.health;
    }

    public bool IsStandard() { return name.StartsWith("Standard "); }

    public override int GetHashCode()
    {
        var hashCode = 869866102;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(type);
        hashCode = hashCode * -1521134295 + count.GetHashCode();
        hashCode = hashCode * -1521134295 + health.GetHashCode();
        return hashCode;
    }
}