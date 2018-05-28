using System.Collections.Generic;
using UnityEngine;

public class Disarm : TacticTrigger
{
    public override void Activate(Location location)
    {
        Piece from = OnEnterGame.gameInfo.board[location];
        Piece into = new Piece(from);
        into.Transform(Collection.StandardCollection(from.GetPieceType()));
        GameController.TransformPiece(from, into);
    }

    public override List<Location> ValidTargets()
    {
        List<Location> targets = new List<Location>();
        foreach (int playerID in new List<int> { OnEnterGame.gameInfo.firstPlayer, OnEnterGame.gameInfo.secondPlayer })
            foreach (Piece piece in OnEnterGame.gameInfo.activePieces[playerID])
                targets.Add(piece.location);
        return targets;
    }
}
