using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorribleLandmine : TacticTrigger {

    public override void Activate(Vector2Int location)
    {
        if(OnEnterGame.gameInfo.Destroyable(location, "Trap"))
            GameController.Eliminate(OnEnterGame.gameInfo.board[location]);
    }
}
