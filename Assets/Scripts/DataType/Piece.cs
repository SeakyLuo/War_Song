using UnityEngine;

[System.Serializable]
public class Piece
{
    public static Vector2Int noLocation = new Vector2Int(-1, -1);
    public Vector2Int location;
    public int oreCost = 0;
    public int health = 0;
    public int freeze = 0;
    public int ownerID;
    public bool active = true;
    public bool original;
    public Collection collection;

    private Vector2Int castle;

    public Piece(Piece piece)
    {
        location = piece.location;
        freeze = piece.freeze;
        active = piece.active;
        ownerID = piece.ownerID;
        oreCost = piece.oreCost;
        health = piece.health;
        collection = piece.collection;
        castle = piece.castle;
        original = piece.original;
    }

    public Piece(string type, Vector2Int loc, bool IsAlly, int owner, bool Original)
    {
        /// Standard Piece
        collection = Collection.standardCollectionDict[type];
        castle = loc;
        location = loc;
        ownerID = owner;
        original = Original;
    }

    public Piece(Collection setupCollection, Vector2Int loc, int OreCost, int owner, bool Original)
    {
        collection = setupCollection;
        castle = loc;
        location = loc;
        oreCost = OreCost;
        health = setupCollection.health;
        ownerID = owner;
        original = Original;
    }

    public string GetName() { return collection.name; }
    public string GetPieceType() { return collection.type; }
    public bool IsStandard() { return collection.name.StartsWith("Standard "); }
    public bool IsAlly() { return ownerID == Login.playerID; }
    public Vector2Int GetCastle() { return castle; }
    public bool IsMinion() { return collection.type != "General"; }
}
