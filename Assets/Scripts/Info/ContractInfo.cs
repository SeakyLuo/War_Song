using UnityEngine;
using UnityEngine.UI;

public class ContractInfo : MonoBehaviour {

    public ContractAttributes attributes;
    public Image image;
    public Text contractName;

	void Start () {
        gameObject.name = attributes.Name;
        contractName.text = attributes.Name;
        image.sprite = attributes.image;
    }

    public void SetAttributes(ContractAttributes contractAttributes)
    {
        attributes = contractAttributes;
        image.sprite = attributes.image;
        contractName.text = attributes.Name;
    }

}
