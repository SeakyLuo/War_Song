using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Trigger", menuName = "TacticTrigger")]
public class TacticTrigger : ScriptableObject
{
    public bool needsTarget = true;
    public int afterRound = 0;
    public int lastRound = 1; // Number of rounds this effect will last
    public int effectiveStarts = 0; // Effective round number. 0 if effective immediately, 
    [HideInInspector] public int oreCost;
    [HideInInspector] public int goldCost;
    private Vector2Int target;

    public virtual void Activate() { }  // Override this if !needsTarget
    public virtual void Activate(Vector2Int location) { } // Override this if needsTarget // Trap usually Override this
    public virtual List<Vector2Int> ValidTargets() { return new List<Vector2Int>(); } // Offers the locations of targets
    public virtual void Last() { } // 

    public bool Activatable()
    {
        if (needsTarget) return GameInfo.round >= afterRound && ValidTargets().Count != 0;
        return GameInfo.round >= afterRound;
    }
}
