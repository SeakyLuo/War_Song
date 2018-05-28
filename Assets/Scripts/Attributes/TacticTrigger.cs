using System.Collections.Generic;
using UnityEngine;

//[UnityEngine.CreateAssetMenu(fileName = "Trigger", menuName = "TacticTrigger")]
public class TacticTrigger : ScriptableObject
{
    public bool needsTarget = true; // Number of targets
    public int afterRound = 0;
    public int effectLasts = 1; // Number of rounds this effect will last
    public int effectiveRound = 0; // Effective round number. 0 if effective immediately
    [HideInInspector] public Tactic tactic;
    private Location target;

    public virtual void Activate() { }  // Override this if !needsTarget
    public virtual void Activate(Location location) { } // Override this if needsTarget // Trap usually Override this
    public virtual List<Location> ValidTargets() { return new List<Location>(); } // Offers the locations of targets
    public virtual void EndOfGame() { }
    public virtual void Last() { } // 
    public virtual bool Activatable()
    {
        if (needsTarget) return OnEnterGame.gameInfo.round >= afterRound && ValidTargets().Count != 0;
        return OnEnterGame.gameInfo.round >= afterRound;
    }
}
