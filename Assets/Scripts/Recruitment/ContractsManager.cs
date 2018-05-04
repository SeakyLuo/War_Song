using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContractsManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static List<string> contractName = new List<string>()
    {
        "Standard Contract", "Artillery Seller", "Human Resource", "Animal Smuggler", "Wise Elder"
    };
    public static bool warned = false;

    public Canvas parentCanvas;
    public GameObject dragContract;
    public GameObject dragHere, claimCards, use10Contracts, cardView;
    public Text use10ContractsText, congratsText;
    public List<GameObject> contractSlots = new List<GameObject>();

    private GameObject targetContract;

    // Use this for initialization
    private void Start()
    {
        foreach (KeyValuePair<string, int> pair in InfoLoader.user.contracts)
            if (pair.Value != 0)
                contractSlots[contractName.IndexOf(pair.Key)].GetComponent<PlayerContract>().SetCount(pair.Value);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject selectedObject = eventData.pointerCurrentRaycast.gameObject;
        bool contractDragged = (selectedObject.tag == "Contract"),
             countDragged = (selectedObject.name == "CountPanel");
        if (contractDragged || countDragged)
        {
            dragContract.SetActive(true);
            dragContract.transform.position = Input.mousePosition;
            dragHere.SetActive(true);
            if (contractDragged)
            {
                targetContract = selectedObject.transform.parent.gameObject;
                dragContract.GetComponent<PlayerContract>().SetAttributes(targetContract.GetComponent<PlayerContract>().attributes);
                targetContract.GetComponent<PlayerContract>().SetCount(InfoLoader.user.contracts[targetContract.name] - 1);
                dragContract.GetComponent<PlayerContract>().SetCount(1);
            }
            else
            {
                targetContract = selectedObject.transform.parent.parent.gameObject;
                dragContract.GetComponent<PlayerContract>().SetAttributes(targetContract.GetComponent<PlayerContract>().attributes);
                int count = InfoLoader.user.contracts[targetContract.name];
                if(count > 10)
                {
                    targetContract.GetComponent<PlayerContract>().SetCount(InfoLoader.user.contracts[targetContract.name] - 10);
                    dragContract.GetComponent<PlayerContract>().SetCount(10);
                }
                else
                {
                    targetContract.GetComponent<PlayerContract>().SetCount(0);
                    dragContract.GetComponent<PlayerContract>().SetCount(count);
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragHere.activeSelf) return;
        dragContract.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragHere.activeSelf) return;
        dragContract.SetActive(false);
        dragHere.SetActive(false);
        if (Input.mousePosition.y >= GetComponent<RectTransform>().rect.height)
        {
            int count = dragContract.GetComponent<PlayerContract>().GetCount();
            congratsText.text = string.Format("Congratulations! You have recruited {0} new allies!", count * 5);
            ResizeCardView(count);
            if (count == 1)
            {
                --InfoLoader.user.contracts[targetContract.name];
                // retrieve cards from the server and set them
                claimCards.SetActive(true);
            }
            else
            {
                if (warned)
                {
                    Use10Contracts();
                }
                else
                {
                    use10Contracts.SetActive(true);
                    if (count == 10) use10ContractsText.text = "Do you want to use 10 contracts?";
                    else use10ContractsText.text = "Do you want to use all contracts?";
                }
            }
        }
        else CancelUse10Contract();
    }

    public void AddContract(ContractAttributes contractAttributes, int contractsCount)
    {
        InfoLoader.user.contracts[contractAttributes.contractName] += contractsCount;
        contractSlots[contractName.IndexOf(contractAttributes.contractName)].GetComponent<PlayerContract>().SetCount(InfoLoader.user.contracts[contractAttributes.contractName]);
    }

    public void CancelUse10Contract()
    {
        use10Contracts.SetActive(false);
        targetContract.GetComponent<PlayerContract>().SetCount(InfoLoader.user.contracts[targetContract.name]);
        warned = false;
    }

    public void Use10Contracts()
    {
        use10Contracts.SetActive(false);
        claimCards.SetActive(true);
        warned = true;
    }

    public void ResizeCardView(int count)
    {
        GridLayoutGroup gridLayoutGroup = cardView.GetComponent<GridLayoutGroup>();
        cardView.GetComponent<RectTransform>().sizeDelta = new Vector2
        (
             cardView.GetComponent<RectTransform>().rect.width,
             gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom + gridLayoutGroup.cellSize.y * count + gridLayoutGroup.spacing.y * (count - 1)
        );
        for (int i = 0; i < 50; i++)
            cardView.transform.Find("Card" + i.ToString()).gameObject.SetActive(i < 5 * count);
    }
}
