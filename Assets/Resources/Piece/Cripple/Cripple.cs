public class Cripple : Trigger
{
    public override void Revenge()
    {
        GameController.PlaceTrap(piece.location, Database.RandomTrap(), Login.playerID);
    }
}
