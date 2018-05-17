public class Revenger : Trigger {

    public override void Revenge()
    {
        GameController.Eliminate(GameInfo.board[piece.location]);
    }
}
