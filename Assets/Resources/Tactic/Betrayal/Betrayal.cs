using System.Collections.Generic;
using UnityEngine;

public class Betrayal: TacticTrigger
{
    public override void Activate(Location location)
    {
        Piece enemy = OnEnterGame.gameInfo.board[location];
        // Gain Control
        GameController.ChangeSide(location, Login.playerID);
        // Join your ally
        if(!OnEnterGame.gameInfo.board[location].IsStandard())
            Login.user.AddCollection(new Collection(enemy.GetName(), enemy.GetPieceType(), 1, enemy.health));
    }

    public override List<Location> ValidTargets()
    {
        List<Location> target = new List<Location>();
        foreach (Piece piece in OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()])
            if(piece.IsMinion())
                target.Add(piece.location);
        return target;
    }
}
