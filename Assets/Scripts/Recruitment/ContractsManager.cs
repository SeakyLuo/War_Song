using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContractsManager : MonoBehaviour {

    public List<GameObject> contractSlots = new List<GameObject>();
    public static List<string> contractName = new List<string>()
    {
        "Standard Contract", "Artillery Seller", "Human Resource", "Animal Smuggler", "Wise Elder"
    };

    // Use this for initialization
    private void Start()
    {
        foreach(KeyValuePair<string, int> pair in InfoLoader.user.contracts)
            if (pair.Value != 0)
                SetContract(contractSlots[contractName.IndexOf(pair.Key)], pair.Value);
    }

    private void SetContract(GameObject contract, int count)
    {
        if (count == 0)
        {
            contract.SetActive(false);
            return;
        }
        else contract.SetActive(true);
        Transform counter = contract.transform.Find("Count");
        Color color = counter.GetComponent<Image>().color;
        if (count > 1)
        {
            counter.Find("Text").GetComponent<Text>().text = count.ToString();
            color.a = 255;
        }
        else
        {
            counter.Find("Text").GetComponent<Text>().text = "";
            color.a = 0;
        }
        counter.GetComponent<Image>().color = color;
    }

    public void AddContract(ContractAttributes contractAttributes, int contractsCount)
    {
        InfoLoader.user.contracts[contractAttributes.contractName] += contractsCount;
        int count = InfoLoader.user.contracts[contractAttributes.contractName];
        GameObject contract = contractSlots[contractName.IndexOf(contractAttributes.contractName)];
        SetContract(contract, count);
    }

    public void ContractTakesEffect(GameObject contract)
    {
        int count = --InfoLoader.user.contracts[contract.name];
        SetContract(contract, count);
        // Show Cards
    }
	
}
