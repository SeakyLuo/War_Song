using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Mission", menuName = "Mission")]
public class Mission: ScriptableObject {

    public string challengeName;
    [TextArea(2, 3)]
    public string description;
    public int progress;    // a of a/b
    public int requirement; // b of a/b
    public int reward;

}