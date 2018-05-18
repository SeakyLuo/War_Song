using System.Collections.Generic;
using UnityEngine;

//[UnityEngine.CreateAssetMenu(fileName = "Trigger", menuName = "PieceTrigger")]
public class Trigger: ScriptableObject {

    public int effectiveRound = 1;
    public int limitedUse = -1; // -1 if unlimited
    public bool activatable = false;
    public bool bloodThirsty = false;
    public bool afterMove = false;
    public bool inEnemyRegion = false;
    public bool inEnemyPalace = false;
    public bool inEnemyCastle = false;
    public bool atEnemyBottom = false;
    public List<string> cantBeDestroyedBy = new List<string>();
    [HideInInspector] public bool silenced = false;
    [HideInInspector] public Piece piece;

    protected bool link = false;

    public Trigger()
    {
        activatable = false;
        limitedUse = -1;
        silenced = false;
        link = false;
    }

    public virtual void StartOfGame() { }
    public virtual void Activate() { }  // Override this if NO targets required
    public virtual void Activate(Vector2Int location) { } // Override this if targets required
    public virtual void Revenge() { } // triggered when eliminated
    public virtual void BloodThirsty() { } // triggered when kills someone
    public virtual List<Vector2Int> ValidLocs(bool link = false) { return MovementController.ValidLocs(piece.location.x, piece.location.y, piece.GetPieceType(), link); }
    public virtual List<Vector2Int> ValidTargets() { return new List<Vector2Int>(); }  // Offers the location of targets
    public virtual void Passive() { } // For instance, your tacitics cost 1 Ore less
    public virtual void StartOfTurn() { }
    public virtual void EndOfTurn() { }
    public virtual void AfterMove() { }
    public virtual void InEnemyRegion() { }
    public virtual void InEnemyPalace() { }
    public virtual void InEnemyCastle() { }
    public virtual void AtEnemyBottom() { }
    public virtual void EndOfGame() { }

    public bool ReceiveMesseage(string message)
    {
        if (message == "BloodThirsty") return bloodThirsty;
        if (message == "AfterMove") return afterMove;
        if (message == "InEnemyRegion") return inEnemyRegion;
        if (message == "InEnemyPalace") return inEnemyPalace;
        if (message == "InEnemyCastle") return inEnemyCastle;
        if (message == "AtEnemyBottom") return atEnemyBottom;
        return false;
    }
    public bool Link() { link = MovementController.IsLink(piece, ValidLocs(true)); return link; }
    public bool Activatable()
    {
        return limitedUse != 0 && (activatable || Link()); // fuck silence
    }

}