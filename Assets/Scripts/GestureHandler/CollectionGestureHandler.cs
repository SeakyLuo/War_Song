using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectionGestureHandler : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject createLineupPanel, infoCard;
    public Canvas parentCanvas;

    public static string CARDSLOTPANEL = "CardSlotPanel";
    public static float xscale = Screen.width / 1920, yscale = Screen.width / 1080;

    private BoardInfo boardInfo;
    private bool dragBegins = false;
    private LineupBuilder lineupBuilder;

    private void Start()
    {
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
            infoCard.transform.position = AdjustedMousePosition();
            infoCard.GetComponent<CardInfo>().SetAttributes(selectedObject.transform.parent.Find("Card").GetComponent<CardInfo>());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragBegins) return;
        infoCard.transform.position = AdjustedMousePosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragBegins) return;
        dragBegins = false;
        CardInfo cardInfo = infoCard.GetComponent<CardInfo>();
        if (InTacticRegion(Input.mousePosition) && cardInfo.GetCardType() == "Tactic")
            lineupBuilder.AddTactic(cardInfo);
        else if (InBoardRegion(Input.mousePosition) && cardInfo.GetCardType() != "Tactic")
            lineupBuilder.AddPiece(cardInfo, Input.mousePosition);
        infoCard.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!createLineupPanel.activeSelf) return;
        GameObject selectedObject = eventData.pointerCurrentRaycast.gameObject;
        if (selectedObject.name == CARDSLOTPANEL)
        {
            CardInfo cardInfo = selectedObject.transform.parent.Find("Card").GetComponent<CardInfo>();
            if (cardInfo.GetCardType() == "Tactic") lineupBuilder.AddTactic(cardInfo);      
            else
            {
                string cardName = cardInfo.GetCardName();
                bool findStandard = false;
                foreach (Vector2Int loc in boardInfo.typeLocations[cardInfo.GetCardType()])
                {
                    Collection oldCollection = boardInfo.cardLocations[loc];
                    if ((cardInfo.IsStandard() && !oldCollection.name.StartsWith("Standard ")) ||
                        (!cardInfo.IsStandard() && oldCollection.name.StartsWith("Standard ")))
                    {
                        findStandard = true;
                        lineupBuilder.AddPiece(cardInfo, loc);
                        break;
                    }
                }
                if (!findStandard)
                {
                    foreach (Vector2Int loc in boardInfo.typeLocations[cardInfo.GetCardType()])
                    {
                        Collection oldCollection = boardInfo.cardLocations[loc];
                        if (cardName != oldCollection.name || cardInfo.GetHealth() != oldCollection.health)
                        {
                            lineupBuilder.AddPiece(cardInfo, loc);
                            break;
                        }
                    }
                }
            }            
        }
    }

    private bool InTacticRegion(Vector2 pos) { return createLineupPanel.activeSelf && 0.75 * Screen.width <= pos.x && pos.x <= Screen.width && 100 * yscale <= pos.y && pos.y <= 1000 * yscale; }

    private bool InBoardRegion(Vector2 pos) { return 200 * xscale <= pos.x && pos.x <= 1440 * xscale && 10 * yscale <= pos.y && pos.y <= 510 * yscale; }

    public void SetBoardInfo(BoardInfo info) { boardInfo = info; }

    private Vector3 AdjustedMousePosition()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Input.mousePosition, parentCanvas.worldCamera, out mousePosition);
        return mousePosition;
    }
}
