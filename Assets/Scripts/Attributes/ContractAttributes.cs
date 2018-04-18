using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attributes", menuName = "Contract")]
public class ContractAttributes:ScriptableObject {

    public string contractName, description;
    public Sprite image;
    public int price;
    public List<string> cardTypes = new List<string>() { "General", "Advisor", "Elephant", "Horse", "Chariot", "Cannon", "Soldier", "Tactic" };
}
