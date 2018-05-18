using UnityEngine;
using UnityEngine.UI;

public class ActivateCoinStore : MonoBehaviour {

    public GameObject infoPanel;
    public Text coins;

	public void Purchase(int amount)
    {
        // Networking
        Login.user.ChangeCoins(amount);
        coins.text = Login.user.coins.ToString();
    }
}
