using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTrap : TacticTrigger
{
    public override void Activate(Vector2Int location)
    {
        GameController.FreezePiece(location, 2);
    }

}
