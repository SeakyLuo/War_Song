using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Attributes", menuName = "Tactic")]
public class TacticAttributes : ScriptableObject {

    public string Name, description;
    public int goldCost, oreCost;
    public Sprite image;

}
