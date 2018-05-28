using System.Collections.Generic;

public class PlaceAFlag : TacticTrigger
{
    public override void Activate(Location location)
    {
        GameController.PlaceFlag(location, Login.playerID);
    }

    public override List<Location> ValidTargets()
    {
        return MovementController.Unoccupied();
    }

}
