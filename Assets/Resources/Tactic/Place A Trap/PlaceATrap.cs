using System.Collections.Generic;
using UnityEngine;

public class PlaceATrap : TacticTrigger {
    public override void Activate(Location location)
    {
        GameController.PlaceTrap(location, Database.RandomTrap(), Login.playerID);
    }

    public override List<Location> ValidTargets()
    {
        return MovementController.Unoccupied();
    }

}