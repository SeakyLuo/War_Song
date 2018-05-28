using System.Collections.Generic;
using UnityEngine;

public class BoardSetup : MonoBehaviour {

    public BoardAttributes boardAttributes;
    public Transform boardCanvas;
    public Sprite newLocation;
    public Dictionary<Location, GameObject> pieces = new Dictionary<Location, GameObject>(); // is not castle

    public void Setup(Lineup lineup, int ownerID)
    {
        foreach (KeyValuePair<Location,Collection> pair in lineup.cardLocations)
        {
            Location loc = pair.Key;
            if (ownerID != Login.playerID) loc = new Location(boardAttributes.boardWidth - pair.Key.x - 1, boardAttributes.boardHeight - pair.Key.y - 1);
            AddPiece(boardCanvas.Find(loc.ToString() + "/Piece").gameObject, pair.Value, loc, ownerID, true);
        }
    }

    public void AddPiece(Collection collection, Location castle, int ownerID, bool original, bool reactivate = false)
    {
        GameObject pieceObj = Instantiate(boardCanvas.Find((OnEnterGame.gameInfo.activePieces[Login.playerID][0].location) + "/Piece").gameObject, boardCanvas);
        pieceObj.name = "Piece";
        pieceObj.transform.SetParent(boardCanvas.Find(castle.ToString()));
        pieceObj.transform.localPosition = new Vector3(0, 0, -1);
        AddPiece(pieceObj, collection, castle, ownerID, original, reactivate);
    }

    private void AddPiece(GameObject pieceObj, Collection collection, Location castle, int ownerID, bool original, bool reactivate = false)
    {
        pieceObj.GetComponent<PieceInfo>().Setup(collection, castle, ownerID, original);
        pieces.Add(castle, pieceObj);
        OnEnterGame.gameInfo.AddPiece(pieceObj.GetComponent<PieceInfo>().trigger, reactivate, false);
    }

    public void TransformPiece(Location location, Piece piece)
    {
        pieces[location].GetComponent<PieceInfo>().Setup(piece, false);
        OnEnterGame.gameInfo.TransformPiece(location, pieces[location].GetComponent<PieceInfo>().trigger);
    }
}
