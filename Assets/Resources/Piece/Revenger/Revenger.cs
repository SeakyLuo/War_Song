public class Revenger : Trigger {

    public override void Revenge()
    {
        GameController.Eliminate(OnEnterGame.gameInfo.board[piece.location]);
    }
}
