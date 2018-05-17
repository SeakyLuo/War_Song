public class TreasureHorse : Trigger {

    public override void Activate()
    {
        if(link) GameController.ChangeCoin(1);
    }
}
