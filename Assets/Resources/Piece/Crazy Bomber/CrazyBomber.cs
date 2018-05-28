using System.Collections.Generic;
using UnityEngine;

public class CrazyBomber : Trigger {

    public override void Activate(Location location)
    {
        Location prev = piece.location;
        MovementController.Move(piece, prev, location);
        for (int i = prev.x; i < location.x; i++)
            for (int j = prev.y; j <location.y; j++)
                GameController.PlaceTrap(new Location(i, j), Database.RandomTrap(), Login.playerID);
    }

    public override List<Location> ValidTargets()
    {
        return MovementController.ChariotLoc(piece.location.x, piece.location.y);
    }
}
