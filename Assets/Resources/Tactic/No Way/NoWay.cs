using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoWay : TacticTrigger
{
    public override void Activate()
    {
        foreach (Tactic tactic in OnEnterGame.gameInfo.unusedTactics[OnEnterGame.gameInfo.TheOtherPlayer()])
            tactic.oreCost += 5;
    }
}
