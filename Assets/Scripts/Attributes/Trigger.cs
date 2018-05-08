using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Trigger", menuName = "PieceTrigger")]
public class Trigger: ScriptableObject {

    public bool activatable = false;
    public int limitedUse = -1; // -1 if unlimited
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
    public virtual void Activate(Vector2Int loc) { } // Override this if targets required
    public virtual void Revenge() { } // triggered when eliminated
    public virtual void Bloodthirsty() { } // triggered when kills someone
    public virtual void AfterMove() { }
    public virtual List<string> CantBeDestroyedBy() { return new List<string>(); }
    public virtual List<Vector2Int> ValidLoc(bool link = false) { return MovementController.ValidLoc(piece.location.x, piece.location.y, piece.GetPieceType(), link); }
    public virtual List<Vector2Int> ValidTarget() { return new List<Vector2Int>(); }  // Offers the location of targets
    public virtual void StartOfTurn() { }
    public virtual void EndOfTurn() { }
    public virtual void InCastle() { }
    public virtual void InPalace() { }
    public virtual void InEnemyRegion() { }
    public virtual void InEnemyPalace() { }
    public virtual void InEnemyCastle() { }
    public virtual void AtEnemyBottom() { }
    public virtual void EndOfGame() { }

    public bool Link() { link = MovementController.IsLink(piece, ValidLoc(true)); return link; }
    public bool Activatable()
    {
        return limitedUse != 0 && (activatable || Link()); // fuck silence
    }

}