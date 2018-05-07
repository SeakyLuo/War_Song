using UnityEngine;

[System.Serializable]
public class Piece
{
    public static Vector2Int noLocation = new Vector2Int(-1, -1);
    public Vector2Int location;
    public bool active = true;

    private Collection collection;
    private Vector2Int castle;
    private int oreCost = 0;
    private bool isAlly;

    public Piece(string type, Vector2Int loc, bool IsAlly)
    {
        /// Standard Piece
        collection = Collection.standardCollectionDict[type];
        castle = loc;
        location = loc;
        isAlly = IsAlly;
    }

    public Piece(Collection setupCollection, Vector2Int loc, int OreCost, bool IsAlly)
    {
        collection = setupCollection;
        castle = loc;
        location = loc;
        oreCost = OreCost;
        isAlly = IsAlly;
    }

    public string GetName() { return collection.name; }
    public int GetHealth() { return collection.health; }
    public int GetOreCost() { return oreCost; }
    public bool IsStandard() { return collection.name.StartsWith("Standard "); }
    public string GetPieceType() { return collection.type; }
    public Vector2Int GetCastle() { return castle; }
    public bool IsAlly() { return isAlly; }
    public void Resurrect(Vector2Int loc)
    {
        // may be useless
        active = true;
        if (loc == noLocation) location = castle;
        else location = loc;
    }
}
