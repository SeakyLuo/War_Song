using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineupsManager : MonoBehaviour {

    public static int modifyLineup = -1;
    public static int lineupsLimit = 9;

    public GameObject createLineupButton, collectionPanel, createLineupPanel;
    public GameObject lineupView;
    public Text myLineups;
    public BoardManager boardManager;
    public GameObject[] lineupObjects = new GameObject[lineupsLimit];

    private static string CUSTOMLINEUP = "Custom Lineup";
    private int lineupsCount = 0;
    private LineupBuilder lineupBuilder;

    // Use this for initialization
    void Start () {
        lineupsCount = Login.user.lineups.Count;
        if (lineupsCount == lineupsLimit) createLineupButton.SetActive(false);
        for (int i = 0; i < lineupsLimit; i++)
        {
            if (i < lineupsCount)
            {                
                lineupObjects[i].GetComponentInChildren<Text>().text = Login.user.lineups[i].lineupName;
                lineupObjects[i].transform.Find("ImagePanel/Image").GetComponent<Image>().sprite = Database.FindPieceAttributes(Login.user.lineups[i].general).image;
            }
            else lineupObjects[i].SetActive(false);
        }
        myLineups.text = "My Lineups\n" + lineupsCount.ToString() + "/9";
        lineupBuilder = createLineupPanel.GetComponent<LineupBuilder>();
        ResizeLineup();
    }

    public void AddLineup(Lineup lineup)
    {        
        if (modifyLineup == -1)
        {
            // Avoid duplicate Custom Lineups
            if (lineup.lineupName == CUSTOMLINEUP)
            {
                int customLineupCount = 1;
                foreach (Lineup l in Login.user.lineups)
                    if (l.lineupName.StartsWith(CUSTOMLINEUP) && l.boardName == lineup.boardName)
                    {
                        if (l.lineupName != CUSTOMLINEUP && l.lineupName != CUSTOMLINEUP + customLineupCount.ToString())
                            break;
                        customLineupCount++;
                        lineup.lineupName = CUSTOMLINEUP + customLineupCount.ToString();
                    }
            }
            Login.user.AddLineup(lineup);
            lineupObjects[lineupsCount].SetActive(true);
            lineupObjects[lineupsCount].transform.Find("ImagePanel/Image").GetComponent<Image>().sprite = Database.FindPieceAttributes(Login.user.lineups[lineupsCount].general).image;
            lineupObjects[lineupsCount++].GetComponentInChildren<Text>().text = lineup.lineupName;
            myLineups.text = "My Lineups\n" + lineupsCount.ToString() + "/9";
            if (lineupsCount == lineupsLimit) createLineupButton.SetActive(false);
            else createLineupButton.SetActive(true);
            ResizeLineup();
        }
        else
        {
            Login.user.ModifyLineup(lineup, modifyLineup);
            lineupObjects[modifyLineup].transform.Find("ImagePanel/Image").GetComponent<Image>().sprite = Database.FindPieceAttributes(Login.user.lineups[modifyLineup].general).image;
            lineupObjects[modifyLineup].GetComponentInChildren<Text>().text = lineup.lineupName;
            modifyLineup = -1;
        }
    }

    public void DeleteLineup()
    {
        // bug
        if (modifyLineup != -1)
        {
            DeleteLineup(modifyLineup);
            modifyLineup = -1;
        }       
    }

    public void DeleteLineup(int number)
    {
        if (Login.user.lastLineupSelected == number) Login.user.SetLastLineupSelected(-1);
        Login.user.RemoveLineup(number);
        lineupObjects[--lineupsCount].SetActive(false);
        for (int i = number; i < lineupsCount; i++)
            lineupObjects[i].GetComponentInChildren<Text>().text = Login.user.lineups[i].lineupName;
        myLineups.text = "My Lineups\n" + lineupsCount.ToString() + "/9";
        ResizeLineup();
    }

    public void OpenLineup(int number)
    {
        modifyLineup = number;        
        boardManager.LoadBoard(Login.user.lineups[number]);
        createLineupPanel.SetActive(true);
        lineupBuilder.SetLineup(Login.user.lineups[number]);
    }

    private void ResizeLineup()
    {
        GridLayoutGroup gridLayoutGroup = lineupView.GetComponent<GridLayoutGroup>();
        int count = Login.user.lineups.Count + 1;
        if (createLineupButton.activeSelf) count++;
        lineupView.GetComponent<RectTransform>().sizeDelta = new Vector2
        (
             lineupView.GetComponent<RectTransform>().rect.width,
             gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom + gridLayoutGroup.cellSize.y * count + gridLayoutGroup.spacing.y * (count - 1)
        );
    }
}
