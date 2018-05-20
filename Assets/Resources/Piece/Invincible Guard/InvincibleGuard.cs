public class InvincibleGuard : Trigger {

	public override void BloodThirsty ()
	{
        OnEnterGame.gameInfo.Act("move", Login.playerID, 1);
	}
}
