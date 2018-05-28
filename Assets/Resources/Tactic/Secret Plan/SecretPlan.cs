using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretPlan : TacticTrigger
{
    public override void Activate(Location location)
    {
        Piece piece = OnEnterGame.gameInfo.board[location];
        Location castle = OnEnterGame.gameInfo.board[location].GetCastle();
        MovementController.Move(piece, piece.location, new Location(castle.x, MovementController.boardAttributes.boardHeight - 1 - castle.y));
    }

    public override List<Location> ValidTargets()
    {
        List<Location> targets = new List<Location>();
        foreach (Piece piece in OnEnterGame.gameInfo.activePieces[Login.playerID])
            if (piece.GetPieceType() == "Soldier" && OnEnterGame.gameInfo.Destroyable(piece.location, "Tactic"))
                targets.Add(piece.location);
        return targets;
    }
}
