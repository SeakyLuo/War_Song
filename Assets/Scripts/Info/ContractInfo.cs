using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContractInfo : MonoBehaviour {

    public ContractAttributes attributes;
    public Image image;
    public Text contractName;

	void Start () {
        gameObject.name = attributes.contractName;
        contractName.text = attributes.contractName;
        image.sprite = attributes.image;
    }

    public void SetContract(ContractAttributes contract)
    {
        attributes = contract;
        image.sprite = contract.image;
        contractName.text = contract.contractName;
    }

}
