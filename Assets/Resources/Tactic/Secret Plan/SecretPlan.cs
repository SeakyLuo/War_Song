using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretPlan : TacticTrigger
{
    public override void Activate(Vector2Int location)
    {
        Piece piece = OnEnterGame.gameInfo.board[location];
        Vector2Int castle = OnEnterGame.gameInfo.board[location].GetCastle();
        MovementController.Move(piece, piece.location, new Vector2Int(castle.x, MovementController.boardAttributes.boardHeight - 1 - castle.y));
    }

    public override List<Vector2Int> ValidTargets()
    {
        List<Vector2Int> targets = new List<Vector2Int>();
        foreach (Piece piece in OnEnterGame.gameInfo.activePieces[Login.playerID])
            if (piece.GetPieceType() == "Soldier" && OnEnterGame.gameInfo.Destroyable(piece.location, "Tactic"))
                targets.Add(piece.location);
        return targets;
    }
}
