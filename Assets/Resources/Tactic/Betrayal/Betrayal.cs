using System.Collections.Generic;
using UnityEngine;

public class Betrayal: TacticTrigger
{
    public override void Activate(Vector2Int location)
    {
        Piece enemy = OnEnterGame.gameInfo.board[location];
        // Gain Control
        GameController.ChangeSide(location, Login.playerID);
        // Join your ally
        if(!OnEnterGame.gameInfo.board[location].IsStandard())
            Login.user.AddCollection(new Collection(enemy.GetName(), enemy.GetPieceType(), 1, enemy.health));
    }

    public override List<Vector2Int> ValidTargets()
    {
        List<Vector2Int> target = new List<Vector2Int>();
        foreach (Piece piece in OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()])
            if(piece.IsMinion())
                target.Add(piece.location);
        return target;
    }
}
