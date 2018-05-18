public class InvincibleGuard : Trigger {

	public override void BloodThirsty ()
	{
        OnEnterGame.gameInfo.actions [Login.playerID]++;
	}
}
