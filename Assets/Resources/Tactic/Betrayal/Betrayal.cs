using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Betrayal: TacticTrigger
{
    public override void Activate(Vector2Int loc)
    {
        // Gain Control
        Piece enemy = GameInfo.board[loc];
        GameInfo.activeEnemies.Remove(enemy);
        enemy.isAlly = true;
        GameInfo.activeAllies.Add(enemy);
        // Join your ally
        InfoLoader.user.AddCollection(new Collection(enemy.GetName(), enemy.GetPieceType(), 1, enemy.GetHealth()));
    }

    public override List<Vector2Int> ValidTarget()
    {
        List<Vector2Int> target = new List<Vector2Int>();
        foreach (Piece piece in GameInfo.activeEnemies)
            if(piece.IsMinion())
                target.Add(piece.location);
        return target;
    }
}
