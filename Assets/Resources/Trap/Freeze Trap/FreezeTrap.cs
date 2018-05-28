using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTrap : TacticTrigger
{
    public override void Activate(Location Location)
    {
        GameController.FreezePiece(Location, 3);
    }
}
