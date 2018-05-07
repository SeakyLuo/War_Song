using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverPiece : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject infoCard;
    private BoardInfo boardInfo;
    private Vector2Int nameLoc;

    private void Start()
    {
        infoCard = transform.parent.parent.parent.parent.parent.Find("InfoCard").gameObject;
        boardInfo = transform.parent.GetComponent<BoardInfo>();
        nameLoc = InfoLoader.StringToVec2(gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LineupBoardGestureHandler.dragBegins) return;
        infoCard.SetActive(true);
        infoCard.GetComponent<CardInfo>().SetAttributes(boardInfo.cardLocations[nameLoc]);
        // could be closer
        infoCard.transform.position = transform.position - new Vector3(0, infoCard.GetComponent<RectTransform>().rect.y + GetComponent<RectTransform>().rect.y);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!LineupBoardGestureHandler.dragBegins && infoCard.activeSelf) infoCard.SetActive(false);
    }
}
