using System.Collections.Generic;
using UnityEngine;

public class HorseRider : Trigger {

	public override void Activate (Location location)
	{
        GameController.Eliminate(OnEnterGame.gameInfo.board[location]);
        GameController.boardSetup.pieces[location].GetComponent<PieceInfo>().trigger.validLocations = MovementController.HorseLoc;
    }

	public override List<Location> ValidTargets ()
	{
		List<Location> targets = new List<Location> ();
		foreach (Piece piece in OnEnterGame.gameInfo.activePieces[Login.playerID])
            if (piece.GetPieceType() == "Soldier" && OnEnterGame.gameInfo.Destroyable(piece.location, "Horse"))
                targets.Add(piece.location);
        return targets;
	}
}
