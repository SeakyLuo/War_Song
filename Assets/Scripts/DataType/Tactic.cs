[System.Serializable]
public class Tactic {

    public string tacticName;
    public int oreCost;
    public int goldCost;

    public Tactic(TacticAttributes tacticAttributes)
    {
        tacticName = tacticAttributes.Name;
        oreCost = tacticAttributes.oreCost;
        goldCost = tacticAttributes.goldCost;
    }

    public Tactic(string name, int OreCost, int GoldCost)
    {
        tacticName = name;
        oreCost = OreCost;
        goldCost = GoldCost;
    }
}
