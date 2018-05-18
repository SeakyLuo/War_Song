public class KingsGuard : Trigger {

	public override void Activate ()
	{
		foreach (Piece piece in OnEnterGame.gameInfo.activePieces[Login.playerID])
			if (piece.GetPieceType () == "General")
				GameController.boardSetup.pieces [piece.location].GetComponent<PieceInfo> ().trigger.cantBeDestroyedBy = Database.types;
	}
}
