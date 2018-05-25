using System.Collections.Generic;
using UnityEngine;

public class HorseRider : Trigger {

	public override void Activate (Vector2Int location)
	{
        GameController.Eliminate(OnEnterGame.gameInfo.board[location]);
        GameController.boardSetup.pieces[location].GetComponent<PieceInfo>().trigger.validLocations = MovementController.HorseLoc;
    }

	public override List<Vector2Int> ValidTargets ()
	{
		List<Vector2Int> targets = new List<Vector2Int> ();
		foreach (Piece piece in OnEnterGame.gameInfo.activePieces[Login.playerID])
            if (piece.GetPieceType() == "Soldier" && OnEnterGame.gameInfo.Destroyable(piece.location, "Horse"))
                targets.Add(piece.location);
        return targets;
	}
}
