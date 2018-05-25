using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkBargain : TacticTrigger
{
    public override void Activate()
    {
        GameController.ChangeOre(6);
    }

}
