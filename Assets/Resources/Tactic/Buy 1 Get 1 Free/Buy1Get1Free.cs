public class Buy1Get1Free : TacticTrigger {

    public override void Activate()
    {
        if (GameController.ChangeCoin(-5))
        {
            GameController.ChangeCoin(10);
        }
    }
}
