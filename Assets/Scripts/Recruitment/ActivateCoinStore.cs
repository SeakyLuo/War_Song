using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateCoinStore : MonoBehaviour {

    public GameObject infoPanel;
    public Text coins;

	public void purchase(int amount)
    {
        // Networking
        InfoLoader.user.coins += amount;
        coins.text = InfoLoader.user.coins.ToString();
    }

    public void ShowInfo()
    {
        infoPanel.SetActive(true);
    }

    public void GotIt()
    {
        infoPanel.SetActive(false);
    }
}
