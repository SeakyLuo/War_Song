using System.Collections.Generic;
using UnityEngine;

public class WisestElder : Trigger
{
    public override void Activate()
    {
        List<Tactic> usedTactics = OnEnterGame.gameInfo.usedTactics[Login.playerID];
        GameController.AddTactic(usedTactics[Random.Range(0, usedTactics.Count)]);
    }
}
