public class TheOreKing : Trigger {

    public override void StartOfGame()
    {
        OnEnterGame.gameInfo.ChangeOre(10);
    }
}
