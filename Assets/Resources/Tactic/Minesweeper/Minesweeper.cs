using System.Collections.Generic;

public class Minesweeper : TacticTrigger
{
    public override void Activate()
    {
        foreach (var trap in new Dictionary<Location, KeyValuePair<string,int>> (OnEnterGame.gameInfo.traps))
            GameController.RemoveTrap(trap.Key);
    }
}
