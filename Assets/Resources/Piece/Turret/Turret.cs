using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret: Trigger {

    public override List<Vector2Int> ValidLoc(bool link = false)
    {
        if (silenced) return MovementController.ValidLoc(piece.location.x, piece.location.y, piece.GetPieceType());
        return new List<Vector2Int>();
    }

    public override List<Vector2Int> ValidTarget()
    {
        if (silenced) return new List<Vector2Int>();
        return MovementController.CannonTarget(piece.location.x, piece.location.y);
    }

    public override void Activate(Vector2Int loc)
    {
        GameController.Eliminate(loc);
        GameController.ChangeOre(-piece.GetOreCost());
    }
}