using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatSoldier : Trigger
{
	public override void EndOfGame()
    {
        if (piece.active && GameInfo.victory == InfoLoader.playerID)
            InfoLoader.user.ChangeCoins(1);
    }
}
