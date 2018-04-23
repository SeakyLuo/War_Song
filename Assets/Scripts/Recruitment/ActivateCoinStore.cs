using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateCoinStore : MonoBehaviour {

    public Text coins;

	public void purchase(int amount)
    {
        // Networking
        InfoLoader.user.coins += amount;
        coins.text = InfoLoader.user.coins.ToString();
    }
}
