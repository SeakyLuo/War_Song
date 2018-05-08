using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Attributes", menuName = "Tactic")]
public class TacticAttributes : ScriptableObject {

    public string Name;
    [TextArea(2, 3)]
    public string description;
    public int goldCost, oreCost;
    public Sprite image;
    public TacticTrigger trigger;
}
