public class Minesweeper : TacticTrigger
{
    public override void Activate()
    {
        foreach(var trap in OnEnterGame.gameInfo.traps)
            GameController.RemoveTrap(trap.Key);
    }
}
