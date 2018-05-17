using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateContractStore : MonoBehaviour {

    public GameObject popupInputAmountWindow, displayContractsPanel, standardContract, unsuccessfulPurchase, successfulPurchase, notEnoughCoins, invalidAmount;
    public Text priceText, contractsAmount, playerCoinsAmount, contractDescription;
    public InputField inputField;
    public ContractsManager contractsManager;

    private GameObject currentContract;
    private ContractAttributes contractAttributes;
    private int many_contracts = 5, contractsCount = 1;
    private List<GameObject> contracts;

    public void Start()
    {
        contracts = new List<GameObject>();
        ChooseContract(standardContract);
        LoadContract();
    }

    public void ConfirmInput()
    {
        if (inputField.text != "")
        {
            contractsCount = int.Parse(inputField.text);
            if(contractsCount == 0)
            {
                inputField.text = "";
                invalidAmount.SetActive(true);
                return;
            }
            priceText.text = (contractsCount * contractAttributes.price).ToString();
            contractsAmount.text = "Amount: " + contractsCount.ToString();
            int contractsNumber = contracts.Count;
            if (contractsCount > contracts.Count)
            {
                for (int i = contractsNumber; i < Mathf.Min(contractsCount, many_contracts); i++)
                    LoadContract();
            }
            else if (contractsCount < contracts.Count)
            {
                for (int i = contractsNumber - 1; i >= contractsCount; i--)
                {
                    Destroy(contracts[i]);
                    contracts.RemoveAt(i);
                }
            }
        }
        popupInputAmountWindow.SetActive(false);
    }

    public void ChooseContract(GameObject contract)
    {
        currentContract = contract.transform.Find("Image").gameObject;
        contractAttributes = contract.GetComponent<ContractInfo>().attributes;
        priceText.text = (contractAttributes.price * contractsCount).ToString();
        contractDescription.text = contractAttributes.description;
        foreach(GameObject obj in contracts) obj.GetComponent<Image>().sprite = contractAttributes.image;
    }

    public void Purchase()
    {
        int price = int.Parse(priceText.text);
        if (InfoLoader.user.coins >= price)
        {
            InfoLoader.user.ChangeCoins(-price);
            playerCoinsAmount.text = InfoLoader.user.coins.ToString();
            contractsManager.AddContract(contractAttributes, contractsCount);
            successfulPurchase.SetActive(true);
        }
        else
        {
            notEnoughCoins.SetActive(true);
        }
    }

    private void LoadContract()
    {
        GameObject contract = Instantiate(currentContract, displayContractsPanel.transform);
        contract.GetComponent<Image>().sprite = contractAttributes.image;
        contracts.Add(contract);
    }
}
