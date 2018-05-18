using System.Collections.Generic;
using UnityEngine;

public class CrazyBomber : Trigger {

    public override void Activate(Vector2Int location)
    {
        Vector2Int prev = piece.location;
        MovementController.MoveTo(location);
        for (int i = prev.x; i < location.x; i++)
            for (int j = prev.y; j <location.y; j++)
                GameController.PlaceTrap(new Vector2Int(i, j), Database.RandomTrap(), Login.playerID);
    }

    public override List<Vector2Int> ValidTargets()
    {
        return MovementController.ChariotLoc(piece.location.x, piece.location.y);
    }
}
