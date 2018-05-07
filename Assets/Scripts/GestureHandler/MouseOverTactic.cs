using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverTactic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject infoCard;

    private Vector3 newPosition;
    private GameObject tactic;

    private void Start()
    {
        newPosition = new Vector3(transform.position.x + GetComponent<RectTransform>().rect.x + infoCard.GetComponent<RectTransform>().rect.x, transform.position.y - 15);
        tactic = transform.Find("Tactic").gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LineupBoardGestureHandler.dragBegins || !tactic.activeSelf) return;
        infoCard.SetActive(true);
        infoCard.GetComponent<CardInfo>().SetAttributes(tactic.GetComponent<TacticInfo>().tactic);
        infoCard.transform.position = newPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!LineupBoardGestureHandler.dragBegins && !TacticGestureHandler.dragBegins && infoCard.activeSelf)
            infoCard.SetActive(false);
    }
}
