using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceAFlag : TacticTrigger
{
    public override void Activate(Vector2Int location)
    {
        GameController.PlaceFlag(location, true);
    }

    public override List<Vector2Int> ValidTargets()
    {
        return MovementController.Unoccupied();
    }

}
