using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkSoldier : Trigger
{
    private bool gained = false;

    public override void Activate()
    {
        if (link)
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
        // better to change description
    }

}
