using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cripple : Trigger
{
    public override void Revenge()
    {
        GameController.PlaceTrap(piece.location, Database.RandomTrap(), InfoLoader.user.playerID);
    }
}
