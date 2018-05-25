public class Revenger : Trigger {

    public override void Revenge()
    {
        if(OnEnterGame.gameInfo.Destroyable(piece.location, "Chariot"))
            GameController.Eliminate(OnEnterGame.gameInfo.board[piece.location]);
    }
}
