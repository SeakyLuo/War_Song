using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TacticGestureHandler : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public Canvas parentCanvas;
    public GameObject collectionPanel, createLineupPanel, card;
    public static string TACTICSLOTPANEL = "TacticSlotPanel";
    public static float xscale = Screen.width / 1920, yscale = Screen.width / 1080;

    private GameObject selectedObject, dragCard, mouseOver, showCardInfo, tactic;
    private bool dragBegins = false;
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
        dragCard = Instantiate(card, parentCanvas.transform);
        dragCard.SetActive(true);
        dragCard.GetComponent<CardInfo>().SetAttributes(tactic.GetComponent<TacticInfo>().tactic);
        dragCard.transform.position = AdjustedMousePosition();
        tactic.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragBegins) dragCard.transform.position = AdjustedMousePosition();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragBegins) return;
        dragBegins = false;
        tactic.SetActive(true);
        if (!InTacticRegion(Input.mousePosition))
            lineupBuilder.RemoveTactic(dragCard.GetComponent<CardInfo>().tactic);        
        Destroy(dragCard);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selectedObject = eventData.pointerCurrentRaycast.gameObject;
        if (selectedObject.name == TACTICSLOTPANEL)
        {
             tactic = selectedObject.transform.parent.Find("Tactic").gameObject;
             if (!tactic.activeSelf)
             {
                collectionManager.SetCurrentPage("Tactic",1);
                collectionManager.ShowCurrentPage();
             }
             else lineupBuilder.RemoveTactic(tactic.GetComponent<TacticInfo>().tactic);
        }            
    }

    private bool InTacticRegion(Vector2 pos) { return createLineupPanel.activeSelf && 0.75 * Screen.width <= pos.x && pos.x <= Screen.width && 100 * yscale <= pos.y && pos.y <= 1000 * yscale; }

    private Vector3 AdjustedMousePosition()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Input.mousePosition, parentCanvas.worldCamera, out mousePosition);
        return mousePosition;
    }
}
