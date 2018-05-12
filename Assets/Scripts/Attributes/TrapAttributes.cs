using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Attributes", menuName = "Trap")]
public class TrapAttributes : ScriptableObject
{
    public string Name;
    [TextArea(2, 3)]
    public string description;
    public Sprite image;
    public TacticTrigger trigger;
}
