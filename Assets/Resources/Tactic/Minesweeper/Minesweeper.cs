using System.Collections.Generic;

public class Minesweeper : TacticTrigger
{
    public override void Activate()
    {
        foreach (var trap in new Dictionary<UnityEngine.Vector2Int, KeyValuePair<string,int>> (OnEnterGame.gameInfo.traps))
            GameController.RemoveTrap(trap.Key);
    }
}
