using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorribleLandmine : TacticTrigger {

    public override void Activate(Location Location)
    {
        if(OnEnterGame.gameInfo.Destroyable(Location, "Trap"))
            GameController.Eliminate(OnEnterGame.gameInfo.board[Location]);
    }
}
