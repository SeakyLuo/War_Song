using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatSoldier : Trigger
{
	public override void EndOfGame()
    {
        if (piece.active)
            InfoLoader.user.coins += 1;
    }
}
