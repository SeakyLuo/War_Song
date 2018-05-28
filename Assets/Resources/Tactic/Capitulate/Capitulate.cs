using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capitulate : TacticTrigger
{
    public override void Activate(Location location)
    {
        Piece enemy = OnEnterGame.gameInfo.board[location];
        // Gain Control
        GameController.ChangeSide(location, Login.playerID);
    }

    public override List<Location> ValidTargets()
    {
        List<Location> target = new List<Location>();
        foreach (Piece piece in OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()])
            if (piece.IsMinion())
                target.Add(piece.location);
        return target;
    }
}
