using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="Attributes",menuName = "Piece")]
public class PieceAttributes : ScriptableObject {

    public string Name, type;
    [TextArea(2,3)]
    public string description;
    public int oreCost;
    public int health;
    public Sprite image;
    public Trigger trigger;
}
