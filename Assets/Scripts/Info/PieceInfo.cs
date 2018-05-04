using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PieceInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material red, black;

    private GameObject PieceInfoCard;
    private GameObject card;
    private Piece piece;
    private PieceAttributes pieceAttributes;
    private Vector3 newPosition;

    private void Start()
    {
        PieceInfoCard = GameObject.Find("PieceInfoCard");
        card = PieceInfoCard.transform.Find("Canvas/Card").gameObject;
        float posX = transform.position.x;
        if (posX <=80) posX += 40;
        else if(posX > 80) posX -= 40;
        float posY = transform.position.y;
        if (posY <= 90) posY += 45;
        else if (posY > 90) posY -= 45;
        newPosition = new Vector3(posX, posY, -11.5f);
    }

    public void Setup(Collection collection, Vector2Int loc, bool isAlly)
    {
        piece = new Piece(collection, loc, isAlly);
        pieceAttributes = FindPieceAttributes(collection.name);
        GetComponentInChildren<Image>().sprite = pieceAttributes.image;
        if (isAlly) GetComponent<Renderer>().material = red;
        else GetComponent<Renderer>().material = black;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        card.SetActive(true);
        card.GetComponent<CardInfo>().SetAttributes(pieceAttributes);
        card.GetComponent<CardInfo>().SetIsAlly(piece.IsAlly());
        PieceInfoCard.transform.position = newPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideInfoCard();
    }

    public void HideInfoCard()
    {
        if (card.activeSelf) card.SetActive(false);
    }

    public Piece GetPiece() { return piece; }
    public PieceAttributes GetPieceAttributes() { return pieceAttributes; }
    public string GetPieceType() { return pieceAttributes.type; }
    public void SetLocation(Vector2Int loc) { piece.SetLocation(loc); }
    public bool IsStandard() { return piece.IsStandard(); }

    private PieceAttributes FindPieceAttributes(string name)
    {
        if (name.StartsWith("Standard ")) return InfoLoader.standardAttributes[name];
        return Resources.Load<PieceAttributes>("Pieces/Info/" + name + "/Attributes");
    }
}