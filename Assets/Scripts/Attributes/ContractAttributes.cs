using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Attributes", menuName = "Contract")]
public class ContractAttributes:ScriptableObject {

    public string Name;
    [TextArea(2, 3)]
    public string description;
    public Sprite image;
    public int price;
    public List<string> cardTypes = new List<string>() { "General", "Advisor", "Elephant", "Horse", "Chariot", "Cannon", "Soldier", "Tactic" };
}
