using UnityEngine;
using UnityEngine.UI;

public class UserContract : MonoBehaviour {

    public ContractAttributes attributes;
    public Image image;

	// Use this for initialization
	void Start () {
        gameObject.name = attributes.contractName;
        image.sprite = attributes.image;
	}
}
