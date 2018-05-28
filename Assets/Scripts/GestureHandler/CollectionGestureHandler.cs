using UnityEngine;
using UnityEngine.EventSystems;

public class CollectionGestureHandler : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static string CARDSLOTPANEL = "CardSlotPanel";
    public static bool dragBegins = false;

    public GameObject createLineupPanel, infoCard;

    private BoardInfo boardInfo;
    private LineupBuilder lineupBuilder;
    private CollectionManager collectionManager;
    private Collection remove;

    private void Start()
    {
        collectionManager = GetComponent<CollectionManager>();
        lineupBuilder = createLineupPanel.GetComponent<LineupBuilder>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!createLineupPanel.activeSelf) return;
        GameObject selectedObject = eventData.pointerCurrentRaycast.gameObject;
        if (selectedObject.name == CARDSLOTPANEL)
        {
            dragBegins = true;
            infoCard.SetActive(true);
            infoCard.transform.position = Input.mousePosition;
            CardInfo cardInfo = selectedObject.transform.parent.Find("Card").GetComponent<CardInfo>();
            infoCard.GetComponent<CardInfo>().SetAttributes(cardInfo);
            if (cardInfo.piece != null) remove = new Collection(cardInfo.piece, 1, cardInfo.GetHealth());
            else if (cardInfo.tactic != null) remove = new Collection(cardInfo.GetCardName());
            collectionManager.RemoveCollection(remove);
            collectionManager.ShowCurrentPage();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragBegins) return;
        infoCard.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragBegins) return;
        dragBegins = false;
        CardInfo cardInfo = infoCard.GetComponent<CardInfo>();
        if (TacticGestureHandler.InTacticRegion(Input.mousePosition) && cardInfo.GetCardType() == "Tactic")
        {
            if (!lineupBuilder.AddTactic(cardInfo))
            {
                collectionManager.AddCollection(new Collection(cardInfo));
                collectionManager.ShowCurrentPage();
            }
        }
        else if (LineupBoardGestureHandler.InBoardRegion(Input.mousePosition) && cardInfo.GetCardType() != "Tactic")
        {
            if (!lineupBuilder.AddPiece(cardInfo, Input.mousePosition))
            {
                collectionManager.AddCollection(new Collection(cardInfo));
                collectionManager.ShowCurrentPage();
            }
        }
        else
        {
            collectionManager.AddCollection(remove);
            collectionManager.ShowCurrentPage();
        }
        infoCard.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!createLineupPanel.activeSelf || SetCursor.cursorSwitched) return;
        GameObject selectedObject = eventData.pointerCurrentRaycast.gameObject;
        if (selectedObject.name != CARDSLOTPANEL || !selectedObject.transform.parent.Find("Card").gameObject.activeSelf) return;
        CardInfo cardInfo = selectedObject.transform.parent.Find("Card").GetComponent<CardInfo>();
        if (cardInfo.GetCardType() == "Tactic")
        {
            if (lineupBuilder.AddTactic(cardInfo))
            {
                collectionManager.RemoveCollection(new Collection(cardInfo));
                collectionManager.ShowCurrentPage();
            }
        }   
        else
        {
            string cardName = cardInfo.GetCardName();
            Location location = Location.NoLocation;
            foreach (Location loc in boardInfo.typeLocations[cardInfo.GetCardType()])
            {
                Collection oldCollection = boardInfo.cardLocations[loc];
                if ((cardInfo.IsStandard() && !oldCollection.name.StartsWith("Standard ")) ||
                    (!cardInfo.IsStandard() && oldCollection.name.StartsWith("Standard ")))
                {
                    location = loc;
                    break;
                }
            }
            if (location == Location.NoLocation)
                foreach (Location loc in boardInfo.typeLocations[cardInfo.GetCardType()])
                {
                    Collection oldCollection = boardInfo.cardLocations[loc];
                    if (cardName != oldCollection.name || cardInfo.GetHealth() != oldCollection.health)
                    {
                        location = loc;
                        break;
                    }
                }
            lineupBuilder.AddPiece(cardInfo, location);
            collectionManager.RemoveCollection(boardInfo.cardLocations[location]);
            collectionManager.ShowCurrentPage();
        }
    }

    public void SetBoardInfo(BoardInfo info) { boardInfo = info; }
}
