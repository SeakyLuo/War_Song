using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InfoLoader : MonoBehaviour {

    public static UserInfo user;
    public static List<BoardAttributes> boards = new List<BoardAttributes>();
    public static string switchSceneCaller = "Main";
    public static Dictionary<string, PieceAttributes> standardAttributes = new Dictionary<string, PieceAttributes>();
    public PieceAttributes standardGeneral, standardAdvisor, standardElephant, standardHorse, standardChariot, standardCannon, standardSoldier;

    private void Awake()
    {
        user = new CheatAccount();
        standardAttributes = new Dictionary<string, PieceAttributes>(){
            { "Standard General", standardGeneral },
            { "Standard Advisor", standardAdvisor },
            { "Standard Elephant", standardElephant },
            { "Standard Horse", standardHorse },
            { "Standard Chariot", standardChariot },
            { "Standard Cannon", standardCannon },
            { "Standard Soldier", standardSoldier }
        };
    }

    // Use this for initialization
    void Start () {
        foreach (string path in Directory.GetDirectories("Assets/Resources/Board/Info"))
            boards.Add(Resources.Load<BoardAttributes>("Board/Info/" + path.Substring(path.IndexOf("Info") + 5) + "/Attributes"));
        boards = Sorted(boards);
    }

    // Needs Sort By Last Use
    private List<BoardAttributes> Sorted(List<BoardAttributes> boardAttributes)
    {
        List<BoardAttributes> newList = boardAttributes;
        BoardAttributes standardBoard = boardAttributes[0];
        foreach (BoardAttributes attribute in boardAttributes)
        {
            if (attribute.boardName == "Standard Board")
            {
                standardBoard = attribute;
                newList.Remove(attribute);
                break;
            }
        }
        newList.OrderBy(BoardAttributes => BoardAttributes.boardName);
        newList.Insert(0, standardBoard);
        return newList;
    }
}