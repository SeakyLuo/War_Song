using System.Collections.Generic;
using UnityEngine;

public class BoardSetup : MonoBehaviour {

    public BoardAttributes boardAttributes;
    public Transform boardCanvas;
    public Sprite newLocation;
    public Dictionary<Vector2Int, GameObject> pieces = new Dictionary<Vector2Int, GameObject>(); // is not castle

    public void Setup(Lineup lineup, bool isAlly)
    {
        foreach (KeyValuePair<Vector2Int,Collection> pair in lineup.cardLocations)
        {
            Vector2Int loc = pair.Key;
            if (!isAlly) loc = new Vector2Int(boardAttributes.boardWidth - pair.Key.x - 1, boardAttributes.boardHeight - pair.Key.y - 1);
            AddPiece(boardCanvas.Find(Database.Vec2ToString(loc) + "/Piece").gameObject, pair.Value, loc, isAlly, true);
        }
    }

    public void AddPiece(Collection collection, Vector2Int castle, bool isAlly, bool original, bool reactivate = false)
    {
        GameObject pieceObj = Instantiate(boardCanvas.Find(Database.Vec2ToString(OnEnterGame.gameInfo.activePieces[Login.playerID][0].location) + "/Piece").gameObject, boardCanvas);
        pieceObj.transform.SetParent(boardCanvas.Find(Database.Vec2ToString(castle)));
        pieceObj.transform.localPosition = new Vector3(0, 0, -1);
        AddPiece(pieceObj, collection, castle, isAlly, original, reactivate);
    }

    private void AddPiece(GameObject pieceObj, Collection collection, Vector2Int castle, bool isAlly, bool original, bool reactivate = false)
    {
        pieceObj.GetComponent<PieceInfo>().Setup(collection, castle, isAlly, original);
        pieces.Add(castle, pieceObj);
        OnEnterGame.gameInfo.AddPiece(pieceObj.GetComponent<PieceInfo>().piece, reactivate);
    }
}
