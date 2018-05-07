using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardInfo : MonoBehaviour
{
    public BoardAttributes attributes;
    public Dictionary<Vector2Int, Collection> cardLocations = new Dictionary<Vector2Int, Collection>();
    public Dictionary<string, PieceAttributes> attributesDict = new Dictionary<string, PieceAttributes>();
    public Dictionary<string, List<Vector2Int>> typeLocations = new Dictionary<string, List<Vector2Int>>();
    public Dictionary<string, string> locationType = new Dictionary<string, string>();
    public Dictionary<Vector2Int, Collection> standardLocations;

    private void Awake()
    {
        if (standardLocations == null) SetupStandardLocation();
        DataSetup();
        GameObject.Find("Collection").GetComponent<CollectionGestureHandler>().SetBoardInfo(this);
        transform.parent.parent.parent.GetComponent<LineupBuilder>().SetBoardInfo(this);
        transform.parent.GetComponent<LineupBoardGestureHandler>().SetBoardInfo(this);
    }

    public void Reset()
    {
        SetAttributes(attributes);
    }

    private void SetupStandardLocation()
    {
        standardLocations = new Dictionary<Vector2Int, Collection>
        {
            {attributes.agloc, Collection.General },
            {attributes.aaloc1, Collection.Advisor },{attributes.aaloc2, Collection.Advisor },
            {attributes.aeloc1, Collection.Elephant },{attributes.aeloc2, Collection.Elephant },
            {attributes.ahloc1, Collection.Horse },{attributes.ahloc2, Collection.Horse },
            {attributes.arloc1, Collection.Chariot },{attributes.arloc2, Collection.Chariot },
            {attributes.acloc1, Collection.Cannon },{attributes.acloc2, Collection.Cannon },
            {attributes.asloc1, Collection.Soldier },{attributes.asloc2, Collection.Soldier },
            {attributes.asloc3, Collection.Soldier },{attributes.asloc4, Collection.Soldier },
            {attributes.asloc5, Collection.Soldier }
        };
    }

    public void SetAttributes(string boardName, Dictionary<Vector2Int, Collection> newLocations)
    {
        SetAttributes(Resources.Load<BoardAttributes>("Board/" + boardName + "/Attributes"), newLocations);
    }

    public void SetAttributes(BoardAttributes board, Dictionary<Vector2Int, Collection> newLocations = null)
    {
        attributes = board;
        if (standardLocations == null) SetupStandardLocation();
        DataSetup(newLocations);
        Color tmpColor;
        foreach (KeyValuePair<string, PieceAttributes> pair in attributesDict)
        {
            Image image = gameObject.transform.Find(pair.Key).Find("CardImage").GetComponent<Image>();
            image.sprite = pair.Value.image;
            tmpColor = image.color;
            tmpColor.a = 255;
            image.color = tmpColor;
        }
    }

    public void SetStandardCard(string type, Vector2Int location)
    {
        SetCard(Collection.standardCollectionDict[type], location);
    }

    public void SetCard(Collection collection, Vector2Int location)
    {
        collection.count = 1;
        cardLocations[location] = collection;
        string locName = InfoLoader.Vec2ToString(location);
        attributesDict[locName] = LoadPieceAttributes(collection.name);
    }

    public void SetCard(PieceAttributes attributes, Vector2Int location)
    {
        cardLocations[location] = new Collection(attributes.Name, attributes.type, 1, attributes.health);
        attributesDict[InfoLoader.Vec2ToString(location)] = attributes;
    }

    private void DataSetup(Dictionary<Vector2Int, Collection> newLocations = null)
    {
        if (newLocations == null)
        {
            newLocations = standardLocations;
            cardLocations = newLocations;
        }
        else
        {
            cardLocations = new Dictionary<Vector2Int, Collection>
            {
                { attributes.agloc, newLocations[attributes.agloc] },
                { attributes.aaloc1,newLocations[attributes.aaloc1] },{ attributes.aaloc2,newLocations[attributes.aaloc2] },
                { attributes.aeloc1,newLocations[attributes.aeloc1] },{ attributes.aeloc2,newLocations[attributes.aeloc2] },
                { attributes.ahloc1,newLocations[attributes.ahloc1] },{ attributes.ahloc2,newLocations[attributes.ahloc2] },
                { attributes.arloc1,newLocations[attributes.arloc1] },{ attributes.arloc2,newLocations[attributes.arloc2] },
                { attributes.acloc1,newLocations[attributes.acloc1] },{ attributes.acloc2,newLocations[attributes.acloc2] },
                { attributes.asloc1,newLocations[attributes.asloc1] },{ attributes.asloc2,newLocations[attributes.asloc2] },
                { attributes.asloc3,newLocations[attributes.asloc3] },{ attributes.asloc4,newLocations[attributes.asloc4] },{ attributes.asloc5,newLocations[attributes.asloc5] },
            };
        }        
        attributesDict = new Dictionary<string, PieceAttributes>
        {
            { InfoLoader.Vec2ToString(attributes.agloc), LoadPieceAttributes(newLocations[attributes.agloc].name) },
            { InfoLoader.Vec2ToString(attributes.aaloc1), LoadPieceAttributes(newLocations[attributes.aaloc1].name) },
            { InfoLoader.Vec2ToString(attributes.aaloc2), LoadPieceAttributes(newLocations[attributes.aaloc2].name) },
            { InfoLoader.Vec2ToString(attributes.aeloc1), LoadPieceAttributes(newLocations[attributes.aeloc1].name) },
            { InfoLoader.Vec2ToString(attributes.aeloc2), LoadPieceAttributes(newLocations[attributes.aeloc2].name) },
            { InfoLoader.Vec2ToString(attributes.ahloc1), LoadPieceAttributes(newLocations[attributes.ahloc1].name) },
            { InfoLoader.Vec2ToString(attributes.ahloc2), LoadPieceAttributes(newLocations[attributes.ahloc2].name) },
            { InfoLoader.Vec2ToString(attributes.arloc1), LoadPieceAttributes(newLocations[attributes.arloc1].name) },
            { InfoLoader.Vec2ToString(attributes.arloc2), LoadPieceAttributes(newLocations[attributes.arloc2].name) },
            { InfoLoader.Vec2ToString(attributes.acloc1), LoadPieceAttributes(newLocations[attributes.acloc1].name) },
            { InfoLoader.Vec2ToString(attributes.acloc2), LoadPieceAttributes(newLocations[attributes.acloc2].name) },
            { InfoLoader.Vec2ToString(attributes.asloc1), LoadPieceAttributes(newLocations[attributes.asloc1].name) },
            { InfoLoader.Vec2ToString(attributes.asloc2), LoadPieceAttributes(newLocations[attributes.asloc2].name) },
            { InfoLoader.Vec2ToString(attributes.asloc3), LoadPieceAttributes(newLocations[attributes.asloc3].name) },
            { InfoLoader.Vec2ToString(attributes.asloc4), LoadPieceAttributes(newLocations[attributes.asloc4].name) },
            { InfoLoader.Vec2ToString(attributes.asloc5), LoadPieceAttributes(newLocations[attributes.asloc5].name) }
        };
        typeLocations = new Dictionary<string, List<Vector2Int>>
        {
            { "General", new List <Vector2Int>{ attributes.agloc } },
            { "Advisor", new List<Vector2Int>{ attributes.aaloc1, attributes.aaloc2} },
            { "Elephant", new List<Vector2Int> { attributes.aeloc1, attributes.aeloc2 } },
            { "Horse", new List<Vector2Int> { attributes.ahloc1, attributes.ahloc2 } },
            { "Chariot", new List<Vector2Int> { attributes.arloc1, attributes.arloc2 } },
            { "Cannon", new List<Vector2Int> { attributes.acloc1, attributes.acloc2 } },
            { "Soldier", new List<Vector2Int> { attributes.asloc1, attributes.asloc2, attributes.asloc3, attributes.asloc4, attributes.asloc5 } }
        };
        locationType = new Dictionary<string, string>
        {
            { InfoLoader.Vec2ToString(attributes.agloc), "General" },
            { InfoLoader.Vec2ToString(attributes.aaloc1), "Advisor" },{ InfoLoader.Vec2ToString(attributes.aaloc2), "Advisor" },
            { InfoLoader.Vec2ToString(attributes.aeloc1), "Elephant" },{ InfoLoader.Vec2ToString(attributes.aeloc2), "Elephant" },
            { InfoLoader.Vec2ToString(attributes.ahloc1), "Horse" },{ InfoLoader.Vec2ToString(attributes.ahloc2), "Horse" },
            { InfoLoader.Vec2ToString(attributes.arloc1), "Chariot" },{ InfoLoader.Vec2ToString(attributes.arloc2), "Chariot" },
            { InfoLoader.Vec2ToString(attributes.acloc1), "Cannon" },{ InfoLoader.Vec2ToString(attributes.acloc2), "Cannon" },
            { InfoLoader.Vec2ToString(attributes.asloc1), "Soldier" },{ InfoLoader.Vec2ToString(attributes.asloc2), "Soldier" },
            { InfoLoader.Vec2ToString(attributes.asloc3), "Soldier" },{ InfoLoader.Vec2ToString(attributes.asloc4), "Soldier" },{ InfoLoader.Vec2ToString(attributes.asloc5), "Soldier" }
        };
    }

    private static PieceAttributes LoadPieceAttributes(string pieceName)
    {
        if (pieceName.StartsWith("Standard ")) return InfoLoader.standardAttributes[pieceName];
        return Resources.Load<PieceAttributes>("Pieces/" + pieceName + "/Attributes");
    }
}