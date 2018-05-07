using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TacticGestureHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public GameObject collectionPanel, createLineupPanel, infoCard;
    public static string TACTICSLOTPANEL = "TacticSlotPanel";
    public static bool dragBegins = false;

    private GameObject selectedObject, mouseOver, showCardInfo, tactic;
    private LineupBuilder lineupBuilder;
    private CollectionManager collectionManager;

    private void Start()
    {
        lineupBuilder = createLineupPanel.GetComponent<LineupBuilder>();
        collectionManager = collectionPanel.GetComponent<CollectionManager>();
    }

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

    public void OnPointerClick(PointerEventData eventData)
    {
        selectedObject = eventData.pointerCurrentRaycast.gameObject;
        if (selectedObject.name == TACTICSLOTPANEL)
        {
             tactic = selectedObject.transform.parent.Find("Tactic").gameObject;
             if (!tactic.activeSelf) collectionManager.SetCurrentPage("Tactic",1);
             else lineupBuilder.RemoveTactic(tactic.GetComponent<TacticInfo>().tactic);
        }            
    }

    public static bool InTacticRegion(Vector2 pos)
    {
        return 0.75 * Screen.width <= pos.x;
    }
}
