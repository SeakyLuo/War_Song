public class Buy1Get1Free : TacticTrigger {

    public override void Activate()
    {
        GameController.ChangeCoin(10);
    }
}
