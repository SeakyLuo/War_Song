using UnityEngine;

public class FrightenBomb : TacticTrigger {

    public override void Activate(Location Location)
    {
        GameController.ChangePieceHealth(Location, -1);
    }
}
