using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalLegacy : TacticTrigger
{
    public override void EndOfGame()
    {
        if (OnEnterGame.gameInfo.victory != Login.playerID)
            Login.user.ChangeCoins(5);
    }

    public override bool Activatable()
    {
        return false;
    }
}
