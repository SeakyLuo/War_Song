using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverPiece : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{    
    public GameObject card;
    public static float xscale = Screen.width / 1920, yscale = Screen.width / 1080;

    private bool dragBegins;
    private GameObject parentCanvas, showCardInfo, dragCard;
    private BoardInfo boardInfo;
    private Vector2Int nameLoc;
    private Color tmpColor;

    private void Start()
    {
        parentCanvas = GameObject.Find("Canvas");
        boardInfo = transform.parent.GetComponent<BoardInfo>();
        nameLoc = StringToVec2(gameObject.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        showCardInfo = Instantiate(card, parentCanvas.transform);
        showCardInfo.GetComponent<CardInfo>().SetAttributes(boardInfo.cardLocations[nameLoc]);
        showCardInfo.transform.position = AdjustedMousePosition() + new Vector3(-150 * xscale, 150 * yscale, 0);
    }

    // Doesn't work as expected.
    public void OnPointerExit(PointerEventData eventData)
    {
        if (showCardInfo != null) Destroy(showCardInfo);
    }

    private Vector2Int StringToVec2(string loc) { return new Vector2Int((int)Char.GetNumericValue(loc[0]), (int)Char.GetNumericValue(loc[1])); }

    private Vector3 AdjustedMousePosition()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Input.mousePosition, parentCanvas.GetComponent<Canvas>().worldCamera, out mousePosition);
        return mousePosition;
    }
}
