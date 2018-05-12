using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trigger", menuName = "TacticTrigger")]
public class HorribleLandmine : TacticTrigger {

    public override void Activate(Vector2Int location)
    {
        GameController.Eliminate(GameInfo.board[location]);
    }
}
