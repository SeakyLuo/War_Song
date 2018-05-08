using System.Collections.Generic;
using UnityEngine;

public class BoardSetup : MonoBehaviour {

    public BoardAttributes boardAttributes;
    public Transform boardCanvas;
    public Dictionary<Vector2Int, GameObject> pieces = new Dictionary<Vector2Int, GameObject>(); // is not castle

    public void Setup(Lineup lineup, bool isAlly)
    {
        foreach (KeyValuePair<Vector2Int,Collection> pair in lineup.cardLocations)
        {
            Vector2Int loc = pair.Key;
            if (!isAlly) loc = new Vector2Int(boardAttributes.boardWidth - pair.Key.x - 1, boardAttributes.boardHeight - pair.Key.y - 1);
            AddPiece(boardCanvas.Find(InfoLoader.Vec2ToString(loc) + "/Piece").gameObject, pair.Value, loc, isAlly);
        }
    }

    public void AddPiece(Collection collection, Vector2Int castle, bool isAlly)
    {
        GameObject pieceObj = Instantiate(boardCanvas.Find(InfoLoader.Vec2ToString(GameInfo.activeAllies[0].location) + "/Piece").gameObject);
        pieceObj.transform.SetParent(boardCanvas.Find(InfoLoader.Vec2ToString(castle)));
        pieceObj.transform.localPosition = new Vector3(0, 0, 0);
        AddPiece(pieceObj, collection, castle, isAlly);
    }

    private void AddPiece(GameObject pieceObj, Collection collection, Vector2Int castle, bool isAlly)
    {
        pieceObj.GetComponent<PieceInfo>().Setup(collection, castle, isAlly);
        pieces.Add(castle, pieceObj);
        Piece piece = pieceObj.GetComponent<PieceInfo>().piece;
        GameInfo.Add(piece);
        List<Piece> piecesWithCastle;
        if (GameInfo.castles.TryGetValue(piece.GetCastle(), out piecesWithCastle))
            GameInfo.castles[piece.GetCastle()].Add(piece);
        else GameInfo.castles.Add(piece.GetCastle(), new List<Piece> { piece });
    }
}
