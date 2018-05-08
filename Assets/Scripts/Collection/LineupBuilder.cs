using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineupBuilder : MonoBehaviour {
    // Recommend Tactics || Random Tactics

    public GameObject collections, selectBoardPanel, myLineup, copyReminder, fullReminder, sameTacticReminder;
    public Text tacticsCountText, totalOreCostText, totalGoldCostText;
    public InputField inputField;
    public static int tacticsLimit = 10, current_tactics = 0;
    public static Lineup copy = new Lineup();

    private Lineup lineup = new Lineup();
    private CollectionManager collectionManager;
    private LineupsManager lineupsManager;
    private Transform board, lineupBoard;
    private BoardInfo boardInfo;
    private GameObject[] tacticObjs;
    private List<TacticAttributes> tacticAttributes = new List<TacticAttributes>();    
    private int totalOreCost = 0, totalGoldCost = 0;
    private Vector3 mousePosition;

    private void Awake()
    {
        collectionManager = collections.GetComponent<CollectionManager>();
        tacticObjs = GameObject.FindGameObjectsWithTag("Tactic");
        foreach (GameObject obj in tacticObjs) obj.SetActive(false);
        lineup.lineupName = "Custom Lineup";
    }

    private void OnEnable()
    {
        collectionManager.SetCardsPerPage(4);
        collectionManager.ShowCurrentPage();
        if (!copy.IsEmpty()) PasteLineup();
    }

    private void OnDisable()
    {
        collectionManager.SetCardsPerPage(8);
        collectionManager.ShowCurrentPage();
    }

    void Start()
    {
        lineupsManager = myLineup.GetComponent<LineupsManager>();
        board = transform.Find("BoardPanel/Board");
        lineupBoard = board.Find("LineupBoard(Clone)");
        boardInfo = lineupBoard.GetComponent<BoardInfo>();
    }

    public void AddPiece(CardInfo cardInfo, Vector2Int loc)
    {
        PieceAdder(cardInfo, loc, loc.x, loc.y);
    }

    public void AddPiece(CardInfo cardInfo, Vector3 loc)
    {
        Vector2Int location = LineupBoardGestureHandler.FindLoc(loc);
        string cardType,
               locName = InfoLoader.Vec2ToString(location);
        if (boardInfo.locationType.TryGetValue(locName, out cardType) && cardType == cardInfo.GetCardType())
            PieceAdder(cardInfo, location, location.x, location.y);
    }

    private void PieceAdder(CardInfo cardInfo, Vector2Int loc, int locx, int locy)
    {
        string locName = locx.ToString() + locy.ToString();
        if (lineupBoard == null) lineupBoard = board.transform.Find("LineupBoard(Clone)");
        Collection newCollection = new Collection(cardInfo.piece, 1, cardInfo.GetHealth());
        if (newCollection.type == "General") lineup.general = newCollection.name;
        lineup.cardLocations[loc] = newCollection;
        lineupBoard.Find(locName).Find("CardImage").GetComponent<Image>().sprite = cardInfo.image.sprite;
        collectionManager.RemoveCollection(newCollection);
        collectionManager.AddCollection(boardInfo.cardLocations[loc]);
        collectionManager.ShowCurrentPage();
        boardInfo.SetCard(newCollection, loc);        
    }

    public void AddTactic(CardInfo cardInfo)
    {
        if (current_tactics == tacticsLimit)
        {
            StartCoroutine(FullReminder());
            return;
        }
        else if (InTactics(cardInfo.GetCardName()))
        {
            StartCoroutine(SameTacticReminder());
            return;
        }
        TacticAdder(cardInfo.tactic);
        collectionManager.RemoveCollection(new Collection(cardInfo.GetCardName()));
        collectionManager.ShowCurrentPage();
    }

    private void AddTactic(string tacticName)
    {
        // called by progrommer
        TacticAdder(InfoLoader.FindTacticAttributes(tacticName));
    }

    private void TacticAdder(TacticAttributes attributes)
    {
        totalOreCost += attributes.oreCost;
        totalGoldCost += attributes.goldCost;
        int index = 0;
        if (current_tactics == 0 || LessThan(attributes, tacticAttributes[0])) index = 0;
        else if (GreaterThan(attributes, tacticAttributes[current_tactics - 1])) index = current_tactics;
        else
        {
            for (int i = 0; i < current_tactics - 1; i++)
            {
                if (GreaterThan(attributes, tacticAttributes[i]) && LessThan(attributes, tacticAttributes[i + 1]))
                {
                    index = i + 1;
                    break;
                }
            }
        }
        lineup.tactics.Insert(index, attributes.Name);
        tacticAttributes.Insert(index, attributes);
        tacticObjs[current_tactics++].SetActive(true);
        for (int i = index; i < current_tactics; i++)
            tacticObjs[i].GetComponent<TacticInfo>().SetAttributes(tacticAttributes[i]);
        SetTexts();
    }

    public void RemoveTactic(TacticAttributes attributes)
    {
        // called by user
        if (current_tactics == 0) return;
        TacticRemover(attributes);
        collectionManager.AddCollection(new Collection(attributes.Name));
        collectionManager.ShowCurrentPage();
    }
    
    private void RemoveTactic(string TacticName)
    {
        TacticRemover(tacticAttributes[lineup.tactics.IndexOf(TacticName)]);
    }

    private void TacticRemover(TacticAttributes attributes)
    {
        int index = lineup.tactics.IndexOf(attributes.Name);
        totalOreCost -= attributes.oreCost;
        totalGoldCost -= attributes.goldCost;
        if (current_tactics > 1)
        {
            for (int i = index; i < current_tactics - 1; i++)
                tacticObjs[i].GetComponent<TacticInfo>().SetAttributes(tacticAttributes[i + 1]);
        }
        else tacticObjs[0].GetComponent<TacticInfo>().Clear();
        lineup.tactics.Remove(attributes.Name);
        tacticAttributes.RemoveAt(index);                   
        tacticObjs[--current_tactics].SetActive(false);
        SetTexts();        
    }

    private bool LessThan(TacticAttributes attributes1, TacticAttributes attributes2)
    {
        // attributes1 less than attributes2
        return attributes1.oreCost < attributes2.oreCost ||
              (attributes1.oreCost == attributes2.oreCost && attributes1.goldCost < attributes2.goldCost) ||
              (attributes1.oreCost == attributes2.oreCost && attributes1.goldCost == attributes2.goldCost && attributes1.Name.CompareTo(attributes2.Name) < 0);
    }

    private bool GreaterThan(TacticAttributes attributes1, TacticAttributes attributes2)
    {
        // Because tactics can't be the same.
        return !LessThan(attributes1, attributes2);
    }

    private bool InTactics(string attributes)
    {
        foreach (string name in lineup.tactics) if (name == attributes) return true;
        return false;
    }

    private IEnumerator SameTacticReminder()
    {
        sameTacticReminder.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        sameTacticReminder.SetActive(false);
    }

    private IEnumerator FullReminder()
    {
        fullReminder.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        fullReminder.SetActive(false);
    }

    private void SetTexts()
    {
        totalOreCostText.text = totalOreCost.ToString();
        totalGoldCostText.text = totalGoldCost.ToString();
        tacticsCountText.text = "Tactics\n" + lineup.tactics.Count.ToString() + "/10";
    }

    public void RenameLineup()
    {
        lineup.lineupName = inputField.text;
    }

    private void ResumeCollections()
    {
        collectionManager.RemoveStandardCards();
        collectionManager.SetCardsPerPage(8);
        gameObject.SetActive(false);
        selectBoardPanel.GetComponent<BoardManager>().DestroyBoard();
    }

    public void DeleteLineup()
    {
        lineupsManager.DeleteLineup();
        // upload to the server
        ResetLineup(true);
        ResumeCollections();
    }

    public void SaveLineup()
    {
        lineup.cardLocations = boardInfo.cardLocations;
        lineup.boardName = boardInfo.attributes.boardName;
        lineup.complete = (lineup.tactics.Count == Lineup.tacticLimit);
        // Incomplete Reminder
        lineupsManager.AddLineup(lineup);
        // upload to the server
        ResetLineup();
        ResumeCollections();
    }

    public void ResetLineup(bool returnCards = false)
    {
        if (returnCards)
        {
            // return cards
            foreach (KeyValuePair<Vector2Int, Collection> pair in lineup.cardLocations)
                collectionManager.AddCollection(pair.Value);
            foreach (string tactic in lineup.tactics)
            {
                RemoveTactic(tactic);
                collectionManager.AddCollection(new Collection(tactic));
            }
            collectionManager.RemoveStandardCards();
            collectionManager.ShowCurrentPage();
        }
        lineup = new Lineup();
        inputField.text = "Custom Lineup";
        current_tactics = totalOreCost = totalGoldCost = 0;
        tacticAttributes.Clear();
        foreach (GameObject obj in tacticObjs) obj.SetActive(false);
        SetTexts();
        //boardInfo.Reset();
    }

    public void CopyLineup()
    {
        copy = lineup;        
        StartCoroutine(CopyReminder());
    }

    private IEnumerator CopyReminder()
    {
        copyReminder.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        copyReminder.SetActive(false);
    }

    public void PasteLineup()
    {
        SetLineup(copy, true);
    }

    public void SetLineup(Lineup newLineup, bool isCopy = false)
    {
        ResetLineup(isCopy);
        lineup.lineupName = newLineup.lineupName;
        inputField.text = newLineup.lineupName;
        lineup.boardName = newLineup.boardName;
        if (isCopy)
        {
            foreach (string tactic in newLineup.tactics)
            {
                if (collectionManager.RemoveCollection(new Collection(tactic)))
                    AddTactic(tactic);
                // can add virtual card like hearthstore
            }
            foreach (KeyValuePair<Vector2Int, Collection> pair in newLineup.cardLocations)
            {
                Collection collection = pair.Value;
                if (!collectionManager.RemoveCollection(collection) || 
                    !collectionManager.RemoveCollection(new Collection(collection.name,collection.type))) //find card with the same name
                {
                    Collection standardCollection = Collection.StandardCollection(collection.type);
                    boardInfo.cardLocations[pair.Key] = standardCollection;
                    lineup.cardLocations[pair.Key] = standardCollection;
                }
                else
                {
                    boardInfo.cardLocations[pair.Key] = newLineup.cardLocations[pair.Key];
                    lineup.cardLocations[pair.Key] = newLineup.cardLocations[pair.Key];
                }
            }
        }
        else
        {
            foreach (string tactic in newLineup.tactics) AddTactic(tactic);
            boardInfo.cardLocations = newLineup.cardLocations;
            lineup.cardLocations = newLineup.cardLocations;
        }
        boardInfo.SetAttributes(newLineup.boardName, boardInfo.cardLocations);
        SetTexts();        
    }

    public void SetBoardInfo(BoardInfo info) { boardInfo = info; }
}
