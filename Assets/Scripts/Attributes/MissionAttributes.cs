using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Mission", menuName = "Mission")]
public class MissionAttributes : ScriptableObject {

    public string missionName;
    [TextArea(2, 3)]
    public string description;
    public int progress;    // a of a/b
    public int requirement; // b of a/b
    public int reward;

}

public class Mission
{
    public string missionName;
    public int progress;

    public Mission(string name, int Progress = 0)
    {
        missionName = name;
        progress = Progress;
    }
}