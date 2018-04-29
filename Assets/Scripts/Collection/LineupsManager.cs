using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineupsManager : MonoBehaviour {

    public GameObject createLineupButton, selectBoardPanel, collectionPanel, createLineupPanel;
    public Text myLineups;
    public static int lineupsLimit = 9;
    public GameObject[] lineupObjects = new GameObject[lineupsLimit];

    private int lineupsCount = 0,
        modifyLineup = -1;
    private static string CUSTOMLINEUP = "Custom Lineup";
    private BoardManager boardManager;
    private LineupBuilder lineupBuilder;

    // Use this for initialization
    void Start () {
        lineupsCount = InfoLoader.user.lineups.Count;
        if (lineupsCount == lineupsLimit) createLineupButton.SetActive(false);
        for (int i = 0; i < lineupsLimit; i++)
        {
            if (i < lineupsCount)
            {                
                lineupObjects[i].GetComponentInChildren<Text>().text = InfoLoader.user.lineups[i].lineupName;
                // Assign Data
            }
            else lineupObjects[i].SetActive(false);
        }
        myLineups.text = "My Lineups\n" + lineupsCount.ToString() + "/9";
        boardManager = selectBoardPanel.GetComponent<BoardManager>();
        lineupBuilder = createLineupPanel.GetComponent<LineupBuilder>();
    }

    public void AddLineup(Lineup lineup)
    {        
        if (modifyLineup == -1)
        {
            // Avoid duplicate Custom Lineups
            if (lineup.lineupName == CUSTOMLINEUP)
            {
                int customLineupCount = 1;
                foreach (Lineup l in InfoLoader.user.lineups)
                    if (l.lineupName.StartsWith(CUSTOMLINEUP) && l.boardName == lineup.boardName)
                    {
                        if (l.lineupName != CUSTOMLINEUP && l.lineupName != CUSTOMLINEUP + customLineupCount.ToString())
                            break;
                        customLineupCount++;
                        lineup.lineupName = CUSTOMLINEUP + customLineupCount.ToString();
                    }
            }
            InfoLoader.user.lineups.Add(lineup);
            lineupObjects[lineupsCount].SetActive(true);
            lineupObjects[lineupsCount++].GetComponentInChildren<Text>().text = lineup.lineupName;
            myLineups.text = "My Lineups\n" + lineupsCount.ToString() + "/9";
            if (lineupsCount == lineupsLimit) createLineupButton.SetActive(false);
            else createLineupButton.SetActive(true);
        }
        else
        {
            InfoLoader.user.lineups[modifyLineup] = lineup;
            lineupObjects[modifyLineup].GetComponentInChildren<Text>().text = lineup.lineupName;
            modifyLineup = -1;
        }
    }

    public void DeleteLineup()
    {
        // bug
        if (modifyLineup != -1)
        {
            if (InfoLoader.user.lastLineupSelected == modifyLineup) InfoLoader.user.lastLineupSelected = -1;
            InfoLoader.user.lineups.RemoveAt(modifyLineup);
            lineupObjects[--lineupsCount].SetActive(false);
            for (int i = modifyLineup; i < lineupsCount; i++)
                lineupObjects[i].GetComponentInChildren<Text>().text = InfoLoader.user.lineups[i].lineupName;
            myLineups.text = "My Lineups\n" + lineupsCount.ToString() + "/9";
            modifyLineup = -1;
        }       
    }

    public void OpenLineup(int number)
    {
        modifyLineup = number;        
        boardManager.LoadBoard(InfoLoader.user.lineups[number]);
        createLineupPanel.SetActive(true);
        lineupBuilder.SetLineup(InfoLoader.user.lineups[number]);
    }

}
