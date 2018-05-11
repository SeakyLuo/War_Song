using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CollectionGestureHandler : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static string CARDSLOTPANEL = "CardSlotPanel";
    public static bool dragBegins = false;

    public GameObject createLineupPanel, infoCard;

    private BoardInfo boardInfo;
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
            infoCard.transform.position = Input.mousePosition;
            infoCard.GetComponent<CardInfo>().SetAttributes(selectedObject.transform.parent.Find("Card").GetComponent<CardInfo>());
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
            lineupBuilder.AddTactic(cardInfo);
        else if (LineupBoardGestureHandler.InBoardRegion(Input.mousePosition) && cardInfo.GetCardType() != "Tactic")
            lineupBuilder.AddPiece(cardInfo, Input.mousePosition);
        infoCard.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!createLineupPanel.activeSelf || SetCursor.cursorSwitched) return;
        GameObject selectedObject = eventData.pointerCurrentRaycast.gameObject;
        if (selectedObject.name != CARDSLOTPANEL || !selectedObject.transform.parent.Find("Card").gameObject.activeSelf) return;
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

    public void SetBoardInfo(BoardInfo info) { boardInfo = info; }
}
