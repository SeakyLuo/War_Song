using System.Collections.Generic;
using UnityEngine;

public class LinkBomber : Trigger {

    public override void Activate(Location location)
    {
        GameController.PlaceTrap(location, Database.RandomTrap(), Login.playerID);
        GameController.onEnterGame.TriggerTrap(location);
    }

    public override List<Location> ValidTargets()
    {
        return MovementController.CannonTarget(piece.location.x, piece.location.y);
    }
}
