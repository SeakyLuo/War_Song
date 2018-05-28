using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret: Trigger {

    public override List<Location> ValidLocs(bool link = false)
    {
        if (silenced) return MovementController.ValidLocs(piece.location.x, piece.location.y, piece.GetPieceType());
        return new List<Location>();
    }

    public override List<Location> ValidTargets()
    {
        return MovementController.CannonTarget(piece.location.x, piece.location.y);
    }

    public override void Activate(Location location)
    {
        GameController.Eliminate(location, piece);
    }
}