using System.Collections.Generic;

public class ProtectTheKing : TacticTrigger
{
    public override void Activate()
    {
        foreach (Piece piece in OnEnterGame.gameInfo.activePieces[Login.playerID])
            if (piece.GetPieceType() == "General"){
                OnEnterGame.gameInfo.triggers[piece.location].cantBeDestroyedBy = new List<string>(Database.types)
                {
                    "Trap"
                };
                break;
            }
    }
}