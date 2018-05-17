using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorribleLandmine : TacticTrigger {

    public override void Activate(Vector2Int location)
    {
        GameController.Eliminate(GameInfo.board[location]);
    }
}
