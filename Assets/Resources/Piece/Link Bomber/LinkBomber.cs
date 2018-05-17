using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkBomber : Trigger {

    public override void Activate(Vector2Int location)
    {
        GameController.PlaceTrap(location, Database.RandomTrap(), InfoLoader.playerID);
        GameController.onEnterGame.TriggerTrap(location);
    }

    public override List<Vector2Int> ValidTargets()
    {
        return MovementController.CannonTarget(piece.location.x, piece.location.y);
    }
}
