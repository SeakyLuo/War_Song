using UnityEngine;

public class FrightenBomb : TacticTrigger {

    public override void Activate(Vector2Int location)
    {
        GameController.ChangePieceHealth(location, -1);
    }
}
