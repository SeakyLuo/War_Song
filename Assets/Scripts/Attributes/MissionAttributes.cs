using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Attributes", menuName = "Mission")]
public class MissionAttributes : ScriptableObject {

    public string Name;
    [TextArea(2, 3)]
    public string description;
    public int requirement; // Like Win 1 Game
    public int reward;
}

public class Mission
{
    public string Name;
    public int progress;

    public Mission(string name, int Progress = 0)
    {
        Name = name;
        progress = Progress;
    }
}