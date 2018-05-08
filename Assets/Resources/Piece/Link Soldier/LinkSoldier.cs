﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkSoldier : Trigger
{
    private bool gained = false;

    public override void Activate()
    {
        if (link && GameController.ChangeOre(-piece.GetOreCost()))
        {
            gained = true;
            limitedUse = 0;
        }
    }

    public override void Revenge()
    {
        if (!gained) return;
        MovementController.Move(piece, piece.location, piece.GetCastle());
        MovementController.KillAt(piece.GetCastle());
        // better to have a mark below
    }

}