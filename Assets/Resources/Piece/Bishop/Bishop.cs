using System.Collections.Generic;
using UnityEngine;

public class Bishop : Trigger {

    public override void Activate(Location location)
    {
        if(link) GameController.AddPiece(Collection.Advisor, location, Login.playerID);
    }

    public override List<Location> ValidTargets()
    {
        List<Location> targets = new List<Location>();
        foreach (Location loc in MovementController.boardAttributes.ElephantCastle())
            if (!OnEnterGame.gameInfo.board.ContainsKey(loc))
                targets.Add(loc);
        return targets;

    }
}