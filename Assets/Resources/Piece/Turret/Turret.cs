using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret: Trigger {

    public override List<Vector2Int> ValidLocs(bool link = false)
    {
        if (silenced) return MovementController.ValidLocs(piece.location.x, piece.location.y, piece.GetPieceType());
        return new List<Vector2Int>();
    }

    public override List<Vector2Int> ValidTargets()
    {
        return MovementController.CannonTarget(piece.location.x, piece.location.y);
    }

    public override void Activate(Vector2Int location)
    {
        GameController.Eliminate(location);
    }
}