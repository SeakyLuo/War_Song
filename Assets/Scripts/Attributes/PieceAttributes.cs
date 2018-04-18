using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Attributes",menuName = "Piece")]
public class PieceAttributes : ScriptableObject {

    public string Name, type, description;
    public int oreCost, health;
    public Sprite image;

}
