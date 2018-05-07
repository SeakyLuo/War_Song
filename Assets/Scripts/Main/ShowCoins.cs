using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCoins : MonoBehaviour {

    public Text coins;
	void Start () {
        coins.text = InfoLoader.user.coins.ToString();
    }
}
