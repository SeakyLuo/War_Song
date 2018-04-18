using UnityEngine;

public class Piece
{
    private Collection collection;
    private Vector2Int startLocation, location;
    private bool isAlly;

    public Piece(Collection setupCollection, Vector2Int loc, bool IsAlly)
    {
        collection = setupCollection;
        startLocation = loc;
        location = loc;
        isAlly = IsAlly;
    }

    public string GetPieceType() { return collection.type; }
    public Vector2Int GetLocation() { return location; }
    public void SetLocation(Vector2Int loc) { location = loc; }
    public void SetStartLocation(Vector2Int loc) { startLocation = loc; }
    public bool IsAlly() { return isAlly; }
}
