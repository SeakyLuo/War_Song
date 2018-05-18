using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreadfulHorse : Trigger {

    public override void Activate(Vector2Int location)
    {
        GameController.Eliminate(OnEnterGame.gameInfo.board[location]);
    }

    public override List<Vector2Int> ValidTargets()
    {
        List<Vector2Int> targets = new List<Vector2Int>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                Vector2Int loc = new Vector2Int(piece.location.x + i, piece.location.x + j);
                if (OnEnterGame.gameInfo.board.ContainsKey(loc) && !OnEnterGame.gameInfo.board[loc].isAlly && OnEnterGame.gameInfo.board[loc].collection.type == "Soldier")
                    targets.Add(loc);
            }
        }
        return targets;
    }
}
