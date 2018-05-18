using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatSoldier : Trigger
{
	public override void EndOfGame()
    {
        if (piece.active && OnEnterGame.gameInfo.victory == Login.playerID)
            Login.user.ChangeCoins(1);
    }
}
