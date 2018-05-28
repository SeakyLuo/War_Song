using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadfulHorse : Trigger {

    public override void Activate(Location location)
    {
        GameController.Eliminate(OnEnterGame.gameInfo.board[location]);
    }

    public override List<Location> ValidTargets()
    {
        List<Location> targets = new List<Location>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                Location loc = new Location(piece.location.x + i, piece.location.x + j);
                if (OnEnterGame.gameInfo.board.ContainsKey(loc)
                    && !OnEnterGame.gameInfo.board[loc].IsAlly()
                    && OnEnterGame.gameInfo.board[loc].collection.type == "Soldier"
                    && OnEnterGame.gameInfo.Destroyable(loc, "Horse"))
                    targets.Add(loc);
            }
        }
        return targets;
    }
}
