using UnityEngine;

[System.Serializable]
public class Piece
{
    public static Vector2Int noLocation = new Vector2Int(-1, -1);
    public Vector2Int location;
    public int oreCost = 0;
    public int health = 0;
    public int freeze = 0;
    public bool active = true;
    public bool isAlly;
    public bool original;
    public Collection collection;

    private Vector2Int castle;

    public Piece(Piece piece)
    {
        location = piece.location;
        freeze = piece.freeze;
        active = piece.active;
        isAlly = piece.isAlly;
        oreCost = piece.oreCost;
        health = piece.health;
        collection = piece.collection;
        castle = piece.castle;
        original = piece.original;
    }

    public Piece(string type, Vector2Int loc, bool IsAlly, bool Original)
    {
        /// Standard Piece
        collection = Collection.standardCollectionDict[type];
        castle = loc;
        location = loc;
        isAlly = IsAlly;
        original = Original;
    }

    public Piece(Collection setupCollection, Vector2Int loc, int OreCost, bool IsAlly, bool Original)
    {
        collection = setupCollection;
        castle = loc;
        location = loc;
        oreCost = OreCost;
        health = setupCollection.health;
        isAlly = IsAlly;
        original = Original;
    }

    public string GetName() { return collection.name; }
    public bool IsStandard() { return collection.name.StartsWith("Standard "); }
    public string GetPieceType() { return collection.type; }
    public Vector2Int GetCastle() { return castle; }
    public bool IsMinion() { return collection.type != "General"; }
}
