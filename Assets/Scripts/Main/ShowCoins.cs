using UnityEngine;
using UnityEngine.UI;

public class ShowCoins : MonoBehaviour {

    public Text coins;
	void Start () {
        coins.text = Login.user.coins.ToString();
    }
}
