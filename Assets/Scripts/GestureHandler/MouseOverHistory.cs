using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseOverHistory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image background;
    public Sprite allyBackground;
    public Sprite enemyBackground;
    public Image cardImage;
    public GameObject historyPanel, eventCard, targetCard;
    public GameEvent gameEvent;

    public void SetAttributes(GameEvent game_event)
    {
        gameEvent = game_event;
        if (gameEvent.eventPlayerID == Login.playerID) background.sprite = allyBackground;
        else background.sprite = enemyBackground;
        SetCard(eventCard, gameEvent.eventTriggerName, gameEvent.eventPlayerID);
        if(gameEvent.eventPlayerID != -1) SetCard(targetCard, gameEvent.targetTriggerName, gameEvent.targetPlayerID);
    }

    public void SetCard(GameObject card, string triggerName, int triggerPlayerID)
    {
        if (Database.pieceList.Contains(gameEvent.targetTriggerName))
            card.GetComponent<CardInfo>().SetAttributes(Database.FindPieceAttributes(triggerName), triggerPlayerID);
        else if (Database.tacticList.Contains(gameEvent.targetTriggerName))
            card.GetComponent<CardInfo>().SetAttributes(Database.FindPieceAttributes(triggerName), triggerPlayerID);
        else if(Database.trapList.Contains(gameEvent.targetTriggerName))
            card.GetComponent<TrapInfo>().SetAttributes(Database.FindTrapAttributes(triggerName), triggerPlayerID);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        historyPanel.SetActive(true);
        if (gameEvent.eventPlayerID != -1) targetCard.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        historyPanel.SetActive(false);
        if (gameEvent.eventPlayerID != -1) targetCard.SetActive(false);
    }
}
