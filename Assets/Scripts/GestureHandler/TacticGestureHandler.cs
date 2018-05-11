using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TacticGestureHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static string TACTICSLOTPANEL = "TacticSlotPanel";
    public static bool dragBegins = false;

    public GameObject infoCard;
    public LineupBuilder lineupBuilder;
    public CollectionManager collectionManager;

    private GameObject selectedObject, showCardInfo, tactic;

    public void OnBeginDrag(PointerEventData eventData)
    {
        selectedObject = eventData.pointerCurrentRaycast.gameObject;
        if (selectedObject.name != TACTICSLOTPANEL) return;
        tactic = selectedObject.transform.parent.Find("Tactic").gameObject;
        if (!tactic.activeSelf) return;
        dragBegins = true;
        infoCard.SetActive(true);
        infoCard.transform.position = Input.mousePosition;
        infoCard.GetComponent<CardInfo>().SetAttributes(tactic.GetComponent<TacticInfo>().tactic);
        lineupBuilder.RemoveTactic(tactic.GetComponent<TacticInfo>().tactic);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragBegins) infoCard.transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragBegins) return;
        dragBegins = false;
        tactic.SetActive(true);
        if (InTacticRegion(Input.mousePosition)) lineupBuilder.AddTactic(infoCard.GetComponent<CardInfo>());
        infoCard.SetActive(false);
    }

    public static bool InTacticRegion(Vector2 pos)
    {
        return 0.75 * Screen.width <= pos.x;
    }
}
