using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceATrap : TacticTrigger {
    public override void Activate(Vector2Int location)
    {
        GameController.PlaceTrap(location, Database.RandomTrap(), InfoLoader.user.playerID);
    }

    public override List<Vector2Int> ValidTargets()
    {
        return MovementController.Unoccupied();
    }

}