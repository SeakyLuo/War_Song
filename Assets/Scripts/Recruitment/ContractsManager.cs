using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContractsManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static List<string> contractName = new List<string>() // can't use database.contractlist because of order
    {
        "Standard Contract", "Artillery Seller", "Human Resource", "Animal Smuggler", "Wise Elder"
    };
    public static bool warned = false;

    public Canvas parentCanvas;
    public GameObject dragContract;
    public GameObject dragHere, claimCards, use10Contracts, cardView;
    public Text use10ContractsText, congratsText;
    public List<GameObject> contractSlots = new List<GameObject>();
    public Texture2D dragCursor;
    public ScrollRect scrollRect;

    private GameObject targetContract;
    private int contractCount;

    // Use this for initialization
    private void Start()
    {
        foreach (KeyValuePair<string, int> pair in Login.user.contracts)
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
            Cursor.SetCursor(dragCursor, Vector2.zero, CursorMode.Auto);
            dragContract.SetActive(true);
            dragContract.transform.position = Input.mousePosition;
            dragHere.SetActive(true);
            if (contractDragged)
            {
                targetContract = selectedObject.transform.parent.gameObject;
                dragContract.GetComponent<PlayerContract>().SetAttributes(targetContract.GetComponent<PlayerContract>().attributes);
                targetContract.GetComponent<PlayerContract>().ChangeCount(-1);
                dragContract.GetComponent<PlayerContract>().SetCount(1);
                contractCount = 1;
            }
            else
            {
                targetContract = selectedObject.transform.parent.parent.gameObject;
                dragContract.GetComponent<PlayerContract>().SetAttributes(targetContract.GetComponent<PlayerContract>().attributes);
                int count = Login.user.contracts[targetContract.name];
                if(count > 10)
                {
                    targetContract.GetComponent<PlayerContract>().ChangeCount(-10);
                    dragContract.GetComponent<PlayerContract>().SetCount(10);
                    contractCount = 10;
                }
                else
                {
                    targetContract.GetComponent<PlayerContract>().SetCount(0);
                    dragContract.GetComponent<PlayerContract>().SetCount(count);
                    contractCount = count;
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragHere.activeSelf) return;
        dragContract.transform.position = Input.mousePosition;
        Cursor.SetCursor(dragCursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragHere.activeSelf) return;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        dragContract.SetActive(false);
        dragHere.SetActive(false);
        if (Input.mousePosition.y >= GetComponent<RectTransform>().rect.height)
        {
            congratsText.text = string.Format("Congratulations! You have recruited {0} new allies!", contractCount * 5);
            ResizeCardView(contractCount);
            if (contractCount == 1)
            {
                // retrieve cards from the server and set them
                SetCards();
                DisplayCards();
            }
            else
            {
                if (warned) Use10Contracts();
                else
                {
                    use10Contracts.SetActive(true);
                    if (contractCount == 10) use10ContractsText.text = "Do you want to use 10 contracts?";
                    else use10ContractsText.text = "Do you want to use all contracts?";
                }
            }
        }
        else CancelUse10Contract();
    }

    public void AddContract(ContractAttributes contractAttributes, int contractsCount)
    {
        Login.user.ChangeContracts(contractAttributes.Name, contractsCount);
        contractSlots[contractName.IndexOf(contractAttributes.Name)].GetComponent<PlayerContract>().SetCount(Login.user.contracts[contractAttributes.Name]);
    }

    public void CancelUse10Contract()
    {
        use10Contracts.SetActive(false);
        targetContract.GetComponent<PlayerContract>().SetCount(Login.user.contracts[targetContract.name]);
        warned = false;
    }

    public void Use10Contracts()
    {
        SetCards();
        use10Contracts.SetActive(false);
        DisplayCards();
        warned = true;
    }

    public void DisplayCards()
    {
        claimCards.SetActive(true);
        scrollRect.verticalNormalizedPosition = 1;
    }

    public void SetCards()
    {
        Login.user.ChangeContracts(targetContract.name, -contractCount, false);
        List<string> types = Database.FindContractAttributes(targetContract.name).cardTypes;
        for(int i = 0; i < contractCount * 5; i++)
        {
            string type = types[Random.Range(0, types.Count)];
            Collection collection;
            if(type == "Tactic")
            {
                string tactic = Database.tacticList[Random.Range(0, Database.tacticList.Count)];
                TacticAttributes attributes = Database.FindTacticAttributes(tactic);
                collection = new Collection(attributes);
                cardView.transform.Find("Card" + i.ToString()).GetComponent<CardInfo>().SetAttributes(collection);
            }
            else
            {
                int luck = Random.Range(0, 100);
                List<string> pieces = Database.pieceListDict[type];
                string piece = pieces[Random.Range(0, pieces.Count)];
                PieceAttributes attributes = Database.FindPieceAttributes(piece);
                collection = new Collection(attributes);
                if (luck > 85) collection.health += (int)Mathf.Ceil(collection.health * 0.1f);
                if (luck > 90) collection.health += (int) Mathf.Ceil(collection.health * 0.1f);
                if (luck > 95) collection.health += (int)Mathf.Ceil(collection.health * 0.1f);
                CardInfo cardInfo = cardView.transform.Find("Card" + i.ToString()).GetComponent<CardInfo>();
                cardInfo.SetAttributes(attributes);
                cardInfo.SetHealth(collection.health);
            }
            Login.user.AddCollection(collection, false);
        }
        Login.user.Upload();
        contractCount = 0;
    }

    public void ResizeCardView(int contractCount)
    {
        GridLayoutGroup gridLayoutGroup = cardView.GetComponent<GridLayoutGroup>();
        cardView.GetComponent<RectTransform>().sizeDelta = new Vector2
        (
             cardView.GetComponent<RectTransform>().rect.width,
             gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom + gridLayoutGroup.cellSize.y * contractCount + gridLayoutGroup.spacing.y * (contractCount - 1)
        );
        int amount = 5 * contractCount;
        for (int i = 0; i < 50; i++)
            cardView.transform.Find("Card" + i.ToString()).gameObject.SetActive(i < amount);
    }
}