using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class OnEnterGame : MonoBehaviour, IPointerClickHandler
{
    public static bool gameover = false;

    public GameObject victoryImage, defeatImage, drawImage, settingsPanel, yourTurnImage, notEnoughCoinsImage, notEnoughOresImage;
    public GameObject pathDot, targetDot, oldLocation;
    public Transform tacticBag;
    public Text roundCount, timer, modeName;
    public Text playerName, playerWin, playerRank;
    public Text opponentName, opponentWin, opponentRank;
    public Text oreText, coinText;
    [HideInInspector] public GameObject board;
    [HideInInspector] public BoardSetup boardSetup;

    private Lineup lineup;
    private List<Transform> tacticObjs = new List<Transform>();
    private Dictionary<String, int> credits = new Dictionary<string, int>()
    {
        { "Chariot", 8 }, { "Horse", 4}, {"Elephant", 3}, {"Advisor", 2}, {"General", 10}, {"Cannon", 4}, {"Soldier", 2}
    };

    // Use this for initialization
    void Start () {
        lineup = InfoLoader.user.lineups[InfoLoader.user.lastLineupSelected];
        board = Instantiate(Resources.Load<GameObject>("Board/" + lineup.boardName + "/Board"));
        board.transform.SetSiblingIndex(1);
        boardSetup = board.GetComponent<BoardSetup>();
        boardSetup.Setup(lineup, true);  // Set up Player Lineup
        boardSetup.Setup(new EnemyLineup(), false);  // Set up Enemy Lineup
        // Set up Player Info
        playerName.text = InfoLoader.user.username;
        playerWin.text = "Win%: "+InfoLoader.user.total.percentage.ToString();
        playerRank.text = "Rank: " + InfoLoader.user.rank.ToString();
        // Set up Opponent Info
        opponentName.text = "Opponent";
        opponentWin.text = "Win%: 80.00";
        opponentRank.text = "Rank: 9900";
        // SetupTactics
        for (int i = 0; i < LineupBuilder.tacticsLimit; i++)
        {
            tacticObjs.Add(tacticBag.Find(String.Format("TacticSlot{0}/Tactic", i)));
            tacticObjs[i].GetComponent<TacticInfo>().SetAttributes(InfoLoader.FindTacticAttributes(lineup.tactics[i]));
        }
        modeName.text = InfoLoader.user.lastModeSelected;
        GameInfo.SetOrder(InfoLoader.user.playerID, 100000000);
        GameInfo.SetGameID(1);
        GameInfo.unusedTactics = new List<string>(lineup.tactics);
        foreach (KeyValuePair<Vector2Int, GameObject> pair in boardSetup.pieces)
        {
            Trigger trigger = pair.Value.GetComponent<PieceInfo>().trigger;
            if (trigger != null) trigger.StartOfGame();
        }
        SetOreText();
        SetCoinText();
        StartCoroutine(Timer());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            settingsPanel.SetActive(true);
            MovementController.PutDownPiece();
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameover)
        {
            gameover = false;
            SceneManager.LoadScene("PlayerMatching");
            GameInfo.Clear();
            Destroy(board);
        }
    }

    private IEnumerator Timer()
    {
        while (true)
        {
            if (gameover) break;
            string seconds = (GameInfo.time % 60).ToString();
            if (seconds.Length == 1) seconds = "0" + seconds;
            timer.text = (GameInfo.time / 60).ToString() + ":" + seconds;
            if (GameInfo.time < 15) timer.color = Color.red;
            else timer.color = Color.white;
            yield return new WaitForSeconds(1.0f);
            if (--GameInfo.time < 0) NextTurn();
        }
    }

    public void Victory()
    {        
        victoryImage.SetActive(true);
        InfoLoader.user.total.Win();
        GameOver();
    }

    public void Defeat()
    {
        defeatImage.SetActive(true);
        InfoLoader.user.total.Lost();
        GameOver();
    }

    public void Draw()
    {
        drawImage.SetActive(true);
        InfoLoader.user.total.Draw();
        GameOver();
    }

    public void GameOver()
    {
        GameInfo.time = GameInfo.maxTime;
        gameover = true;
        if (settingsPanel.activeSelf) settingsPanel.SetActive(false);
        foreach(KeyValuePair<Vector2Int,GameObject> pair in boardSetup.pieces)
        {
            Trigger trigger = pair.Value.GetComponent<PieceInfo>().trigger;
            if (trigger != null) trigger.EndOfGame();
        }
        // CalculateNewRank(); // should be done by server
        foreach (KeyValuePair<Vector2Int, Collection> pair in lineup.cardLocations)
        {
            if (GameInfo.IsAllyAlive(pair.Value)) continue;
            if (pair.Value.health > 0 && --pair.Value.health == 0)
            {
                int index = InfoLoader.user.FindCollection(pair.Value);
                InfoLoader.user.collection.RemoveAt(index);
                int next = InfoLoader.user.FindCollectionWithName(pair.Value.name);
                if (next != -1) lineup.cardLocations[pair.Key] = InfoLoader.user.collection[next];
                else lineup.cardLocations[pair.Key] = Collection.StandardCollection(pair.Value.type);
            }
        }
        foreach(string tactic in lineup.tactics)
        {
            if (GameInfo.unusedTactics.Contains(tactic)) continue;
            int index = InfoLoader.user.FindCollectionWithName(tactic);
            if (index != -1)
            {
                if (--InfoLoader.user.collection[index].count == 0)
                    InfoLoader.user.collection.RemoveAt(index);
            }
            else lineup.complete = false;
        }
    }

    public void Concede()
    {
        Defeat();
    }

    private void CalculateNewRank()
    {
        int credit = 0;
        foreach(Piece piece in GameInfo.activeAllies)
            credit += credits[piece.GetPieceType()];
        foreach (Piece piece in GameInfo.inactiveEnemies)
            credit += credits[piece.GetPieceType()];
    }

    public void YourTurn()
    {
        //StartCoroutine(ShowYourTurn());
        GameInfo.actionTaken = false;
        foreach (KeyValuePair<Vector2Int, GameObject> pair in boardSetup.pieces)
        {
            Trigger trigger = pair.Value.GetComponent<PieceInfo>().trigger;
            if (trigger != null) trigger.StartOfTurn();
        }
    }
    private IEnumerator ShowYourTurn(float time = 1.5f)
    {
        yourTurnImage.SetActive(true);
        yield return new WaitForSeconds(time);
        yourTurnImage.SetActive(false);
    }

    public void NotEnoughCoins()
    {
        StartCoroutine(ShowNotEnoughCoins());
    }
    private IEnumerator ShowNotEnoughCoins(float time = 1.5f)
    {
        notEnoughCoinsImage.SetActive(true);
        yield return new WaitForSeconds(time);
        notEnoughCoinsImage.SetActive(false);
    }
    public void NotEnoughOres()
    {
        StartCoroutine(ShowNotEnoughOres());
    }
    private IEnumerator ShowNotEnoughOres(float time = 1.5f)
    {
        notEnoughOresImage.SetActive(true);
        yield return new WaitForSeconds(time);
        notEnoughOresImage.SetActive(false);
    }

    public void NextTurn()
    {
        foreach (KeyValuePair<Vector2Int, GameObject> pair in boardSetup.pieces)
        {
            Trigger trigger = pair.Value.GetComponent<PieceInfo>().trigger;
            if (trigger != null) trigger.EndOfTurn();
        }
        roundCount.text = (++GameInfo.round).ToString();
        if(GameInfo.round == 150)
        {
            Draw();
            return;
        }
        GameInfo.time = GameInfo.maxTime;
        GameInfo.actionTaken = true;
        YourTurn();
    }

    public void SetOreText()
    {
        oreText.text = GameInfo.ores[InfoLoader.user.playerID].ToString();
    }
    public void SetCoinText()
    {
        coinText.text = InfoLoader.user.coins.ToString();
    }
}
