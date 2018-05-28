using System.Collections.Generic;

public class SpaceWitch : Trigger {

    public override void Activate(Location location)
    {
        MovementController.Move(piece, piece.location, location);
    }

    public override List<Location> ValidTargets()
    {
        List<Location> targets = new List<Location>();
        for(int i = MovementController.boardAttributes.palaceLeft; i <= MovementController.boardAttributes.palaceRight; i++)
            for(int j = MovementController.boardAttributes.palaceDown; j <= MovementController.boardAttributes.palaceUp; j++)
            {
                Location target = new Location(i, j);
                if (!OnEnterGame.gameInfo.board.ContainsKey(target)) targets.Add(target);
            }
        return targets;
    }
}
