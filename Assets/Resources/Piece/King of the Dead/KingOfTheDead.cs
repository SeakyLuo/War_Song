using System.Collections.Generic;
using UnityEngine;

public class KingOfTheDead : Trigger {

    public override void Activate(Location location)
    {
        List<Collection> collections = new List<Collection>();
        foreach (Piece piece in OnEnterGame.gameInfo.inactivePieces[Login.playerID])
            if (piece.GetPieceType() == "Soldier")
                collections.Add(piece.collection);
        GameController.ResurrectPiece(collections[Random.Range(0, collections.Count)], location, Login.playerID);
    }

    public override List<Location> ValidTargets()
    {
        List<Location> targets = new List<Location>();
        foreach (Location loc in MovementController.boardAttributes.SoldierCastle())
            if (!OnEnterGame.gameInfo.board.ContainsKey(loc))
                targets.Add(loc);
        return targets;
    }

}
