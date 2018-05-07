using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PieceInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Material red, black;
    [HideInInspector] public Piece piece;
    [HideInInspector] public Trigger trigger;
    [HideInInspector] public bool selected = false;

    private GameObject PieceInfoCard;
    private GameObject card;
    private PieceAttributes pieceAttributes;
    private Vector3 newPosition;

    private void Start()
    {
        PieceInfoCard = GameObject.Find("BoardInfoCard");
        card = PieceInfoCard.transform.Find("Canvas/Card").gameObject;
        float posX = transform.position.x;
        if (posX <=80) posX += 40;
        else if(posX > 80) posX -= 40;
        float posY = transform.position.y;
        if (posY <= 90) posY += 45;
        else if (posY > 90) posY -= 45;
        newPosition = new Vector3(posX, posY, -13); // need to find better position
    }

    public void Setup(Collection collection, Vector2Int loc, bool isAlly)
    {
        pieceAttributes = FindPieceAttributes(collection.name);
        piece = new Piece(collection, loc, pieceAttributes.oreCost, isAlly);
        if (pieceAttributes.trigger != null) trigger = Instantiate(pieceAttributes.trigger);
        if (trigger != null) trigger.piece = piece; // remove the if when all completed
        GetComponentInChildren<Image>().sprite = pieceAttributes.image;
        if (isAlly) GetComponent<Renderer>().material = red;
        else GetComponent<Renderer>().material = black;
    }

    public List<Vector2Int> ValidLoc()
    {
        if (trigger == null) return MovementController.ValidLoc(piece.location.x, piece.location.y, piece.GetPieceType());
        return trigger.ValidLoc(); 
    }

    public List<Vector2Int> ValidTarget()
    {
        if (trigger == null) return new List<Vector2Int>();
        return trigger.ValidTarget();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selected) return;
        card.SetActive(true);
        card.GetComponent<CardInfo>().SetAttributes(pieceAttributes);
        card.GetComponent<CardInfo>().SetIsAlly(piece.IsAlly());
        card.GetComponent<CardInfo>().SetHealth(piece.GetHealth());
        PieceInfoCard.transform.position = newPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selected = false;
        HideInfoCard();
    }

    public void HideInfoCard()
    {
        if (card.activeSelf) card.SetActive(false);
    }
    
    public PieceAttributes GetPieceAttributes() { return pieceAttributes; }
    public string GetPieceType() { return pieceAttributes.type; }
    public void SetLocation(Vector2Int loc) { piece.location = loc; }
    public bool IsStandard() { return piece.IsStandard(); }
    private PieceAttributes FindPieceAttributes(string name)
    {
        if (name.StartsWith("Standard ")) return InfoLoader.standardAttributes[name];
        return Resources.Load<PieceAttributes>("Pieces/" + name + "/Attributes");
    }
}