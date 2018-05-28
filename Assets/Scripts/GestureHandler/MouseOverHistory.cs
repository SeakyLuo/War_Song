using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseOverHistory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image background;
    public Image cardImage;
    public Sprite allyBackground;
    public Sprite enemyBackground;
    public GameObject historyPanel, eventCard, targetCard;
    [HideInInspector] public GameEvent gameEvent;

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
        switch (Database.FindType(gameEvent.targetTriggerName))
        {
            case "Piece":
                card.GetComponent<CardInfo>().SetAttributes(Database.FindPieceAttributes(triggerName), triggerPlayerID);
                break;
            case "Tactic":
                card.GetComponent<CardInfo>().SetAttributes(Database.FindTacticAttributes(triggerName), triggerPlayerID);
                break;
            case "Trap":
                card.GetComponent<CardInfo>().SetAttributes(Database.FindTrapAttributes(triggerName), triggerPlayerID);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // it flashes. Problem is height.
        //historyPanel.SetActive(true);
        //if (gameEvent.eventPlayerID != -1) targetCard.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    //    historyPanel.SetActive(false);
    //    if (gameEvent.eventPlayerID != -1) targetCard.SetActive(false);
    }
}
