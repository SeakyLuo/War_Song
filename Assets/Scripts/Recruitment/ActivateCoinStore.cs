using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateCoinStore : MonoBehaviour {

    public GameObject infoPanel;
    public Text coins;

	public void Purchase(int amount)
    {
        // Networking
        InfoLoader.user.ChangeCoins(amount);
        coins.text = InfoLoader.user.coins.ToString();
    }
}
