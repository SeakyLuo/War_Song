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

    public static bool operator <(TacticAttributes attributes1, TacticAttributes attributes2) { return new Tactic(attributes1) < new Tactic(attributes2); }
    public bool Equals(TacticAttributes attributes) { return new Tactic(this) == new Tactic(attributes); }
    public static bool operator >(TacticAttributes attributes1, TacticAttributes attributes2) { return new Tactic(attributes1) > new Tactic(attributes2); }
}
