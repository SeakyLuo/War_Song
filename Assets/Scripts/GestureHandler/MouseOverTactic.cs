using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverTactic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject infoCard;
    public LineupBuilder lineupBuilder;
    public CollectionManager collectionManager;
    public GameObject tactic;

    private Vector3 newPosition;

    private void Start()
    {
        newPosition = new Vector3(transform.position.x + GetComponent<RectTransform>().rect.x + infoCard.GetComponent<RectTransform>().rect.x, transform.position.y - 15);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LineupBoardGestureHandler.dragBegins || !tactic.activeSelf) return;
        infoCard.SetActive(true);
        infoCard.GetComponent<CardInfo>().SetAttributes(tactic.GetComponent<TacticInfo>().tacticAttributes);
        infoCard.transform.position = newPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!LineupBoardGestureHandler.dragBegins && !TacticGestureHandler.dragBegins && infoCard.activeSelf)
            infoCard.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!tactic.activeSelf) collectionManager.SetCurrentPage("Tactic", 1);
        else lineupBuilder.RemoveTactic(tactic.GetComponent<TacticInfo>().tacticAttributes);
    }
}
