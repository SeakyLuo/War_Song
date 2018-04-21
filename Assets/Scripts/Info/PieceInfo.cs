using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceInfo : MonoBehaviour {

    private Piece piece;
    private PieceAttributes pieceAttributes;

    public void Setup(Collection collection, Vector2Int loc, bool isAlly)
    {
        piece = new Piece(collection, loc, isAlly);
        pieceAttributes = FindPieceAttributes(collection.name);
        gameObject.GetComponentInChildren<Image>().sprite = pieceAttributes.image;
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