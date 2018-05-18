using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceWitch : Trigger {

    public override void Activate(Vector2Int location)
    {
        MovementController.MoveTo(location);
    }

    public override List<Vector2Int> ValidTargets()
    {
        List<Vector2Int> targets = new List<Vector2Int>();
        for(int i = MovementController.boardAttributes.palaceDownLeft.x; i <= MovementController.boardAttributes.palaceUpperRight.x; i++)
            for(int j = MovementController.boardAttributes.palaceDownLeft.y; j <= MovementController.boardAttributes.palaceUpperRight.y; j++)
            {
                Vector2Int target = new Vector2Int(i, j);
                if (!OnEnterGame.gameInfo.board.ContainsKey(target)) targets.Add(target);
            }
        return targets;
    }
}
