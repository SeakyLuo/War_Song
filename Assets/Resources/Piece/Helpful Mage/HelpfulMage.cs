public class HelpfulMage : Trigger {

    public override void Passive(Tactic tactic)
    {
        GameController.ChangeTacticOreCost(tactic.tacticName, -1);
    }

    public override bool PassiveCriteria(Tactic tactic)
    {
        return tactic.ownerID == Login.playerID;
    }
}
