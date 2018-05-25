using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disarm : TacticTrigger
{
    public override void Activate(Vector2Int location)
    {
        Piece piece = OnEnterGame.gameInfo.board[location];
        GameController.AddPiece(Collection.StandardCollection(piece.GetPieceType()), piece.GetCastle(), piece.ownerID);
        GameController.Eliminate(location, false);
    }

    public override List<Vector2Int> ValidTargets()
    {
        List<Vector2Int> targets = new List<Vector2Int>();
        foreach (int playerID in new List<int> { OnEnterGame.gameInfo.firstPlayer, OnEnterGame.gameInfo.secondPlayer })
            foreach (Piece piece in OnEnterGame.gameInfo.activePieces[playerID])
                targets.Add(piece.location);
        return targets;
    }
}
