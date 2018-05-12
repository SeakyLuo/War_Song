using System.Collections.Generic;
using UnityEngine;

public class Betrayal: TacticTrigger
{
    public override void Activate(Vector2Int location)
    {
        Piece enemy = GameInfo.board[location];
        // Gain Control
        GameController.ChangeSide(location, true);
        // Join your ally
        if(!GameInfo.board[location].IsStandard())
            InfoLoader.user.AddCollection(new Collection(enemy.GetName(), enemy.GetPieceType(), 1, enemy.health));
    }

    public override List<Vector2Int> ValidTargets()
    {
        List<Vector2Int> target = new List<Vector2Int>();
        foreach (Piece piece in GameInfo.activePieces[GameInfo.TheOtherPlayer()])
            if(piece.IsMinion())
                target.Add(piece.location);
        return target;
    }
}
