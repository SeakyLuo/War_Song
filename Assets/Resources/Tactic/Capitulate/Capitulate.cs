using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capitulate : TacticTrigger
{
    public override void Activate(Vector2Int location)
    {
        Piece enemy = OnEnterGame.gameInfo.board[location];
        // Gain Control
        GameController.ChangeSide(location, Login.playerID);
    }

    public override List<Vector2Int> ValidTargets()
    {
        List<Vector2Int> target = new List<Vector2Int>();
        foreach (Piece piece in OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()])
            if (piece.IsMinion())
                target.Add(piece.location);
        return target;
    }
}
