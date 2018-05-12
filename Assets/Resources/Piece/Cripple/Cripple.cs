using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cripple : Trigger
{
    public override void Revenge()
    {
        GameController.PlaceTrap(piece.location, Database.trapList[Random.Range(0, Database.traps.Count)], InfoLoader.user.playerID);
    }
}
