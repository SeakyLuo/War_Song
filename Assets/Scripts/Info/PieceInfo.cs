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

    private void Start()
    {
        PieceInfoCard = GameObject.Find("PieceInfoCard");
        card = PieceInfoCard.transform.Find("Canvas/Card").gameObject;
    }

    public void Setup(Piece piece, bool original)
    {
        pieceAttributes = Database.FindPieceAttributes(piece.GetName());
        trigger = Instantiate(pieceAttributes.trigger);
        SetPiece(piece);
        GetComponentInChildren<Image>().sprite = pieceAttributes.image;
        if (piece.IsAlly()) GetComponent<Renderer>().material = red;
        else GetComponent<Renderer>().material = black;
    }

    public void Setup(Collection collection, Location loc, int ownerID, bool original)
    {
        pieceAttributes = Database.FindPieceAttributes(collection.name);
        if (pieceAttributes.trigger == null) Debug.Log(pieceAttributes.Name);
        trigger = Instantiate(pieceAttributes.trigger);
        SetPiece(new Piece(collection, loc, pieceAttributes.oreCost, ownerID, original));
        GetComponentInChildren<Image>().sprite = pieceAttributes.image;
        if (piece.IsAlly()) GetComponent<Renderer>().material = red;
        else GetComponent<Renderer>().material = black;
    }

    public void SetPiece(Piece setup)
    {
        piece = setup;
        trigger.piece = setup;
    }

    public List<Location> ValidLoc()
    {
        if (trigger == null) return MovementController.ValidLocs(piece.location.x, piece.location.y, piece.GetPieceType());
        return trigger.ValidLocs(); 
    }

    public List<Location> ValidTarget()
    {
        if (trigger == null) return new List<Location>();
        return trigger.ValidTargets();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selected || OnEnterGame.gameInfo.gameOver) return;
        card.SetActive(true);
        card.GetComponent<CardInfo>().SetAttributes(pieceAttributes);
        card.GetComponent<CardInfo>().SetPiece(piece);

        Vector3 newPosition = new Vector3();
        float posX = transform.position.x;
        if (posX <= 80) posX += 40;
        else if (posX > 80) posX -= 40;
        float posY = transform.position.y;
        if (posY <= 90) posY += 45;
        else if (posY > 90) posY -= 45;
        newPosition = new Vector3(posX, posY, -13); // need to find better position
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
    public void SetLocation(Location loc) { piece.location = loc; }
    public bool IsStandard() { return piece.IsStandard(); }
}