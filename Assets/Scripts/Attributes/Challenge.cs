using UnityEngine;

[CreateAssetMenu(fileName = "Challenge", menuName = "Challenge")]
public class Challenge: ScriptableObject {

    public string challengeName;
    public string description;
    public int reward;

}