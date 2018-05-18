using System.Collections.Generic;
using UnityEngine;

public class KingOfTheDead : Trigger {

    public override void Activate(Vector2Int location)
    {
        List<Collection> collections = new List<Collection>();
        foreach (Piece piece in OnEnterGame.gameInfo.inactivePieces[Login.playerID])
            if (piece.GetPieceType() == "Soldier")
                collections.Add(piece.collection);
        GameController.ResurrectPiece(collections[Random.Range(0, collections.Count)], location, true);
    }

    public override List<Vector2Int> ValidTargets()
    {
        List<Vector2Int> targets = new List<Vector2Int>();
        foreach (Vector2Int loc in MovementController.boardAttributes.SoldierCastle())
            if (!OnEnterGame.gameInfo.board.ContainsKey(loc))
                targets.Add(loc);
        return targets;
    }

}
