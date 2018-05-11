using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHunter : Trigger
{
    public override void BloodThirsty()
    {
        GameController.ChangeCoin(1);
    }
}
