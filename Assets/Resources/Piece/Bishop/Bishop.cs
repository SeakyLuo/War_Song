using System.Collections.Generic;
using UnityEngine;

public class Bishop : Trigger {

    public override void Activate(Vector2Int location)
    {
        if(link) GameController.AddPiece(Collection.Advisor, location, true);
    }

    public override List<Vector2Int> ValidTargets()
    {
        List<Vector2Int> targets = new List<Vector2Int>();
        foreach (Vector2Int loc in MovementController.boardAttributes.ElephantCastle())
            if (!OnEnterGame.gameInfo.board.ContainsKey(loc))
                targets.Add(loc);
        return targets;

    }
}