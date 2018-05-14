using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {

    public GameObject confirmButton, preferButton, createLineupButton, createLineupPanel, board, askPasteOrNot;
    public Text boardName, boardInformation;
    public Image boardImage;
    public BoardAttributes standardBoardAttributes;
    [HideInInspector] public int currentBoard = 0;
    [HideInInspector] public List<BoardAttributes> boardAttributes;

    private GameObject loadedBoard;

    private void OnEnable()
    {
        if (!LineupBuilder.copy.IsEmpty())
            askPasteOrNot.SetActive(true);
    }

    private void Start()
    {
        boardAttributes = new List<BoardAttributes>() { Database.FindBoardAttributes(InfoLoader.user.preferredBoard) };
        foreach (string boardName in Database.boardList)
            if(boardName != InfoLoader.user.preferredBoard)
                boardAttributes.Add(Database.FindBoardAttributes(boardName));
        DisplayBoardSelectionInterface();
    }

    public void NextBoard()
    {
        ++currentBoard;
        DisplayBoardSelectionInterface();
    }

    public void PreviousBoard()
    {
        --currentBoard;
        DisplayBoardSelectionInterface();
    }

    private void DisplayBoardSelectionInterface()
    {
        boardName.text = boardAttributes[currentBoard].Name;
        boardImage.sprite = boardAttributes[currentBoard].completeImage;
        boardInformation.text = boardAttributes[currentBoard].description;
        if (boardAttributes[currentBoard].available)
        {
            preferButton.SetActive(true);
            confirmButton.SetActive(true);
        }
        else
        {
            preferButton.SetActive(false);
            confirmButton.SetActive(false);
        }            
    }

    public void PreferBoard()
    {
        if (boardAttributes[currentBoard].available)
        {
            InfoLoader.user.SetPreferredBoard(boardAttributes[currentBoard].Name);
        }
    }

    public void ConfirmBoardSelection()
    {
        gameObject.SetActive(false);
        LoadBoard(boardAttributes[currentBoard]);
        createLineupPanel.SetActive(true);
    }

    public void LoadBoard(Lineup lineup)
    {
        LoadBoard(Database.FindBoardAttributes(lineup.boardName), lineup.cardLocations);
    }

    public void LoadBoard(BoardAttributes attributes, Dictionary<Vector2Int, Collection> newLocations = null)
    {
        loadedBoard = Instantiate(Resources.Load<GameObject>("Board/" + attributes.Name + "/LineupBoard"), board.transform);
        loadedBoard.transform.localPosition = new Vector3(0, 0, 0);
        loadedBoard.SetActive(true);
        loadedBoard.GetComponent<BoardInfo>().SetAttributes(attributes, newLocations);
    }

    public void PasteLineup()
    {
        askPasteOrNot.SetActive(false);
        LoadBoard(LineupBuilder.copy);
        ConfirmBoardSelection();
    }

    public void DontPasteLineup()
    {
        askPasteOrNot.SetActive(false);
        LineupBuilder.copy.Clear();
    }

    public void DestroyBoard()
    {
        Destroy(loadedBoard);
    }
}
