[System.Serializable]
public class Tactic {

    public string tacticName;
    public int oreCost;
    public int goldCost;
    public bool original = true;

    public Tactic(TacticAttributes tacticAttributes, bool Original = true)
    {
        tacticName = tacticAttributes.Name;
        oreCost = tacticAttributes.oreCost;
        goldCost = tacticAttributes.goldCost;
        original = true;
    }

    public Tactic(string name, int OreCost, int GoldCost, bool Original = true)
    {
        tacticName = name;
        oreCost = OreCost;
        goldCost = GoldCost;
        original = Original;
    }

    public Tactic(Collection collection)
    {
        tacticName = collection.name;
        oreCost = collection.oreCost;
        goldCost = collection.health;
        original = true;
    }

    public static bool operator < (Tactic tactic1, Tactic tactic2)
    {
        return tactic1.oreCost < tactic2.oreCost ||
                (tactic1.oreCost == tactic2.oreCost && tactic1.goldCost < tactic2.goldCost) ||
                (tactic1.oreCost == tactic2.oreCost && tactic1.goldCost == tactic2.goldCost && tactic1.tacticName.CompareTo(tactic2.tacticName) < 0);
    }

    public static bool operator >(Tactic tactic1, Tactic tactic2) { return !(tactic1 < tactic2); } // Because tactics can't be the same.
}