using System.Collections;
public class Trigger {

    public int afterTurn = 0;

    public virtual void StartOfTheGame() { }
    public virtual void Link() { }
    public virtual void Revenge() { }
    public virtual void Bloodthirsty() { }
    public virtual void Destroyed() { }
    public virtual void Sacrifice() { }
    public virtual void BeforeMove() { }
    public virtual void AfterMove() { }
    public virtual void WhenPlayed() { }
    public virtual void CantBeDestroyed() { }
    public virtual void PossibleMovements() { }
    public virtual void StartOfTheTurn() { }
    public virtual void EndofTheTurn() { }
    public virtual void StartofTheNextRound() { }
    public virtual void InAllyField() { }
    public virtual void InPalace() { }
    public virtual void AtBottom() { }
    public virtual void InEnemyRegion() { }
    public virtual void InEnemyPalace() { }
    public virtual void AtEnemyBottom() { }
    public virtual void EndOfTheGame() { }

}