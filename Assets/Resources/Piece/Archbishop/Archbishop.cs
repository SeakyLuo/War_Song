using System.Collections.Generic;
using UnityEngine;

public class Archbishop : Trigger {

    public override void Activate(Location location)
    {
        GameController.Eliminate(location, piece);
    }

    public override List<Location> ValidTargets()
    {
        List<Location> targets = new List<Location>();
        for (int i = -1; i <= 1; i++){
            for (int j = -1; j <= 1; j++){
                if (i == 0 && j == 0) continue;
                Location loc = new Location(piece.location.x + i, piece.location.x + j);
                if (OnEnterGame.gameInfo.board.ContainsKey(loc) && OnEnterGame.gameInfo.Destroyable(loc, "Elephant"))
                    targets.Add(loc);
            }
        }
        return targets;
    }
}
