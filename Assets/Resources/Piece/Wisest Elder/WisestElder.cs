using System.Collections.Generic;
using UnityEngine;

public class WisestElder : Trigger
{
    public override void Activate()
    {
        List<Tactic> usedTactics = GameInfo.usedTactics[InfoLoader.playerID];
        GameController.AddTactic(usedTactics[Random.Range(0, usedTactics.Count)]);
    }
}
