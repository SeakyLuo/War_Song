using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverPiece : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject card;
    private BoardInfo boardInfo;
    private Vector2Int nameLoc;

    private void Start()
    {
        card = transform.parent.parent.parent.parent.Find("InfoCard").gameObject;
        boardInfo = transform.parent.GetComponent<BoardInfo>();
        nameLoc = StringToVec2(gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        card.SetActive(true);
        card.GetComponent<CardInfo>().SetAttributes(boardInfo.cardLocations[nameLoc]);
        // could be closer
        card.transform.position = transform.localPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (card.activeSelf) card.SetActive(false);
    }

    private Vector2Int StringToVec2(string loc) { return new Vector2Int((int)Char.GetNumericValue(loc[0]), (int)Char.GetNumericValue(loc[1])); }
}
