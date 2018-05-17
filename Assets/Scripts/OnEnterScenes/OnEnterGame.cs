using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class OnEnterGame : MonoBehaviour, IPointerClickHandler
{
    public static int current_tactic = -1;

    public GameInfo gameInfo;
    public GameObject gameStartImage, victoryImage, defeatImage, drawImage, settingsPanel, yourTurnImage, notEnoughCoinsImage, notEnoughOresImage, fullTacticBag, freezeText, winReward;
    public GameObject pathDot, targetDot, oldLocation, explosion, askTriggerPanel;
    public GameObject history, pieceInfoCard , trapInfoCard, showInfoCard, playerFlag, enemyFlag, freezeImage;
    public Transform tacticBag;
    public Button endTurnButton;
    public Text roundCount, timer, modeName;
    public Text playerName, playerWin, playerRank;
    public Text opponentName, opponentWin, opponentRank;
    public Text oreText, coinText, endTurnText;
    [HideInInspector] public GameObject board;
    [HideInInspector] public BoardSetup boardSetup;

    private static Lineup lineup;
    public static List<Transform> tacticObjs;
    private static List<Button> tacticButtons;
    private static List<TacticTrigger> tacticTriggers;
    private static Dictionary<String, int> credits = new Dictionary<string, int>()
    {
        { "Chariot", 8 }, { "Horse", 4}, {"Elephant", 3}, {"Advisor", 2}, {"General", 10}, {"Cannon", 4}, {"Soldier", 2}
    };

    private static Trigger trigger;
    private static string triggerMessage;

    // Use this for initialization
    void Start () {
        lineup = InfoLoader.user.lineups[InfoLoader.user.lastLineupSelected];
        // Set GameInfo
        gameInfo = new GameInfo(lineup, InfoLoader.playerID, new EnemyLineup(), 100000000); // should be downloading a GameInfo
        //GameInfo.JsonToClass();
        InfoLoader.user.SetGameID(GameInfo.gameID);
        board = Instantiate(Resources.Load<GameObject>("Board/" + lineup.boardName + "/Board"));
        board.transform.SetSiblingIndex(1);
        boardSetup = board.GetComponent<BoardSetup>();
        boardSetup.Setup(lineup, true);  // Set up Player Lineup
        boardSetup.Setup(GameInfo.lineups[GameInfo.TheOtherPlayer()], false);  // Set up Enemy Lineup
        // Set up Player Info
        playerName.text = InfoLoader.user.username;
        playerWin.text = "Win%: "+InfoLoader.user.total.percentage.ToString();
        playerRank.text = "Rank: " + InfoLoader.user.rank.ToString();
        // Set up Opponent Info
        opponentName.text = "Opponent";
        opponentWin.text = "Win%: 80.00";
        opponentRank.text = "Rank: 9900";
        modeName.text = InfoLoader.user.lastModeSelected;
        foreach (KeyValuePair<Vector2Int, GameObject> pair in boardSetup.pieces)
        {
            Trigger trigger = pair.Value.GetComponent<PieceInfo>().trigger;
            if (trigger != null) trigger.StartOfGame();
        }
        SetOreText();
        SetCoinText();
        // SetupTactics
        tacticObjs = new List<Transform>();
        tacticButtons = new List<Button>();
        tacticTriggers = new List<TacticTrigger>();
        for (int i = 0; i < LineupBuilder.tacticsLimit; i++)
        {
            Transform tacticSlot = tacticBag.Find(String.Format("TacticSlot{0}", i));
            tacticButtons.Add(tacticSlot.GetComponent<Button>());
            tacticObjs.Add(tacticSlot.Find("Tactic"));
            tacticObjs[i].GetComponent<TacticInfo>().SetAttributes(Database.FindTacticAttributes(lineup.tactics[i].tacticName), true);
            tacticTriggers.Add(tacticObjs[i].GetComponent<TacticInfo>().trigger);
        }
        StartCoroutine(Timer());
        StartCoroutine(GameStartAnimation());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameInfo.gameOver)
        {
            if (victoryImage.activeSelf && InfoLoader.user.winsToday <= UserInfo.maxWinPerDay)
            {
                victoryImage.SetActive(false);
                winReward.SetActive(true);
                InfoLoader.user.ChangeCoins(1);
                return;
            }
            GameInfo.gameOver = false;
            SceneManager.LoadScene("PlayerMatching");
            GameInfo.Clear();
            Destroy(board);
        }
    }

    private IEnumerator GameStartAnimation()
    {
        //gameStartImage.SetActive(true);
        yield return new WaitForSeconds(1f);
        //gameStartImage.SetActive(false);
        YourTurn();
    }

    private IEnumerator Timer()
    {
        while (true)
        {
            if (GameInfo.gameOver) break;
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
        GameInfo.victory = InfoLoader.playerID;
        victoryImage.SetActive(true);
        InfoLoader.user.Win();
        GameOver();
    }
    public void Defeat()
    {
        GameInfo.victory = GameInfo.TheOtherPlayer();
        defeatImage.SetActive(true);
        InfoLoader.user.Lose();
        GameOver();
    }
    public void Draw()
    {
        GameInfo.victory = -1;
        drawImage.SetActive(true);
        InfoLoader.user.Draw();
        GameOver();
    }

    public void GameOver()
    {
        pieceInfoCard.SetActive(false);
        GameInfo.time = GameInfo.maxTime;
        GameInfo.gameOver = true;
        if (settingsPanel.activeSelf) settingsPanel.SetActive(false);
        foreach(KeyValuePair<Vector2Int,GameObject> pair in boardSetup.pieces)
        {
            Trigger trigger = pair.Value.GetComponent<PieceInfo>().trigger;
            if (trigger != null) trigger.EndOfGame();
        }
        // CalculateNewRank(); // should be done by server
        foreach (KeyValuePair<Vector2Int, Collection> pair in lineup.cardLocations)
        {
            bool alive = false;
            foreach(Piece piece in GameInfo.activePieces[InfoLoader.playerID])
                if (piece.original && piece.GetCastle() == pair.Key)
                {
                    alive = true;
                    lineup.cardLocations[pair.Key] = piece.collection;
                    break;
                }
            if (!alive && pair.Value.health > 0 && --pair.Value.health == 0)
            {
                int index = InfoLoader.user.FindCollection(pair.Value);
                InfoLoader.user.RemoveCollection(index);
                int next = InfoLoader.user.FindCollection(pair.Value.name);
                if (next != -1) lineup.cardLocations[pair.Key] = InfoLoader.user.collection[next];
                else lineup.cardLocations[pair.Key] = Collection.StandardCollection(pair.Value.type);
            }
        }
        foreach(Tactic tactic in lineup.tactics)
        {
            if (GameInfo.unusedTactics[InfoLoader.playerID].Contains(tactic)) continue;
            int index = InfoLoader.user.FindCollection(tactic.tacticName);
            if (index != -1) InfoLoader.user.ChangeCollectionCount(index, -1);
            else lineup.complete = false;
        }
        InfoLoader.user.ModifyLineup(lineup, InfoLoader.user.lastLineupSelected);
        // upload
    }

    public void Concede()
    {
        Defeat();
    }

    private void CalculateNewRank()
    {
        int credit = 0;
        foreach(Piece piece in GameInfo.activePieces[InfoLoader.playerID])
            credit += credits[piece.GetPieceType()];
        foreach (Piece piece in GameInfo.inactivePieces[GameInfo.TheOtherPlayer()])
            credit += credits[piece.GetPieceType()];
    }

    public void EndTurn()
    {
        endTurnButton.interactable = false;
        NextTurn();
    }

    public void YourTurn()
    {
        endTurnText.text = "Your Turn";
        //StartCoroutine(ShowYourTurn());
        GameInfo.actions[InfoLoader.playerID] = 1;
        foreach (KeyValuePair<Vector2Int, GameObject> pair in boardSetup.pieces)
        {
            Trigger trigger = pair.Value.GetComponent<PieceInfo>().trigger;
            if (trigger != null) trigger.StartOfTurn();
        }
        SetTacticInteractable();
    }
    private IEnumerator ShowYourTurn(float time = 1.5f)
    {
        yourTurnImage.SetActive(true);
        yield return new WaitForSeconds(time);
        yourTurnImage.SetActive(false);
    }

    public void NextTurn()
    {
        endTurnText.text = "Enemy Turn";
        foreach (KeyValuePair<Vector2Int, GameObject> pair in boardSetup.pieces)
        {
            Piece piece = pair.Value.GetComponent<PieceInfo>().piece;
            Trigger trigger = pair.Value.GetComponent<PieceInfo>().trigger;
            if (trigger != null)
            {
                if (piece.freeze > 0 && --piece.freeze == 0)
                {
                    Destroy(GameController.freezeImages[pair.Key]);
                    GameController.freezeImages.Remove(pair.Key);
                }
                trigger.EndOfTurn();
            }
        }
        roundCount.text = (++GameInfo.round).ToString();
        if (GameInfo.round == 150)
        {
            Draw();
            return;
        }
        if (GameInfo.actions[InfoLoader.playerID] > 1)
        {
            endTurnButton.interactable = true;
            endTurnText.text = "End Turn";
        }
        else
        {
            endTurnButton.interactable = false;
            endTurnText.text = "Your Turn";
        }
        GameInfo.time = GameInfo.maxTime;
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator EnemyTurn()
    {
        // Set Button interactable
        SetTacticInteractable(false);
        endTurnButton.interactable = false;
        endTurnText.text = "Enemy Turn";

        GameEvent gameEvent = new GameEvent();
        // WWWForm
        while (false) // fetching enemy response // should return "" if no response
            yield return new WaitForSeconds(1);
        GameController.DecodeGameEvent(gameEvent);
        // decode gameEvent
        // if trigger enemyCardInfo.GetComponent<CardInfo>().SetAttributes()
        YourTurn();
    }

    public static void CancelTacticHighlight()
    {
        if (current_tactic == -1) return;
        tacticButtons[current_tactic].GetComponent<Image>().sprite = tacticButtons[current_tactic].spriteState.disabledSprite;
        current_tactic = -1;
    }

    public void TriggerTrap(Vector2Int location)
    {
        if (GameInfo.traps.ContainsKey(location))
        {
            GameEvent gameEvent = new GameEvent();
            explosion.transform.position = new Vector3(location.x * MovementController.scale, location.y * MovementController.scale, -3);
            explosion.transform.SetParent(boardSetup.boardCanvas);
            TrapAttributes trap = Database.FindTrapAttributes(GameInfo.traps[location].Key);
            trapInfoCard.GetComponent<TrapInfo>().SetAttributes(trap, GameInfo.traps[location].Value);
            trap.trigger.Activate(location);
            GameInfo.traps.Remove(location);
            // upload
            AddToHistory(gameEvent);
            StartCoroutine(ShowTrapInfo());
        }
    }

    private IEnumerator ShowTrapInfo(float time = 2f)
    {
        trapInfoCard.SetActive(true);
        explosion.SetActive(true);
        yield return new WaitForSeconds(time);
        trapInfoCard.SetActive(false);
        explosion.SetActive(false);
    }

    public void AddTactic(Tactic tactic)
    {
        int count = GameInfo.unusedTactics[InfoLoader.playerID].Count;
        if (count == LineupBuilder.tacticsLimit) StartCoroutine(ShowFullTacticBag());
        else
        {
            GameInfo.AddTactic(tactic);
            tacticObjs[count].parent.gameObject.SetActive(true);
            int index = 0;
            if (count != 0)
            {
                index = GameInfo.unusedTactics[InfoLoader.playerID].IndexOf(tactic);
                for (int i = count; i > index; i--)
                {
                    TacticInfo tacticInfo = tacticObjs[i - 1].GetComponent<TacticInfo>();
                    tacticObjs[i].GetComponent<TacticInfo>().SetAttributes(tacticInfo.tacticAttributes, tacticInfo.tactic.original);
                }
            }
            TacticAttributes tacticAttributes = Database.FindTacticAttributes(tactic.tacticName);
            tacticObjs[index].GetComponent<TacticInfo>().SetAttributes(tacticAttributes, false);
            tacticTriggers.Insert(index, tacticAttributes.trigger);
            SetTacticInteractable();
        }
    }

    public void RemoveTactic(Tactic tactic)
    {
        int index = GameInfo.FindTactic(tactic.tacticName, InfoLoader.playerID);
        int count = GameInfo.unusedTactics[InfoLoader.playerID].Count;
        if (count > 1)
        {
            for (int i = index; i < count - 1; i++)
            {
                TacticInfo tacticInfo = tacticObjs[i + 1].GetComponent<TacticInfo>();
                tacticObjs[i].GetComponent<TacticInfo>().SetAttributes(tacticInfo.tacticAttributes, tacticInfo.tactic.original);
            }
        }
        else tacticObjs[0].GetComponent<TacticInfo>().Clear();
        tacticTriggers.RemoveAt(index);
        tacticObjs[count - 1].parent.gameObject.SetActive(false);
        GameInfo.RemoveTactic(index);
        SetTacticInteractable();
        CancelTacticHighlight();
    }

    public void SetTacticInteractable(bool interactable = true)
    {
        for (int i = 0; i < tacticTriggers.Count; i++)
            tacticButtons[i].interactable = interactable && tacticTriggers[i].Activatable();
    }

    public void ChangeTacticOreCost(int index, int deltaAmount)
    {
        tacticObjs[index].GetComponent<TacticInfo>().ChangeOreCost(deltaAmount);
    }

    public void ChangeTacticGoldCost(int index, int deltaAmount)
    {
        tacticObjs[index].GetComponent<TacticInfo>().ChangeGoldCost(deltaAmount);
    }

    public void AddToHistory(GameEvent gameEvent)
    {

    }

    public void AskTrigger(Piece piece, Trigger trigger_para, string message)
    {
        if (trigger_para.ReceiveMesseage(message) && trigger_para.piece.oreCost <= GameInfo.ores[InfoLoader.playerID])
        {
            GameInfo.actions[InfoLoader.playerID]++;
            trigger = trigger_para;
            triggerMessage = message;
            pieceInfoCard.SetActive(false);
            showInfoCard.SetActive(true);
            showInfoCard.GetComponent<CardInfo>().SetAttributes(piece.collection);
            askTriggerPanel.SetActive(true);
        }
    }
    public void ConfirmTrigger()
    {
        if (triggerMessage == "BloodThirsty") trigger.BloodThirsty();
        else if (triggerMessage == "AfterMove") trigger.AfterMove();
        else if (triggerMessage == "InEnemyCastle") trigger.InEnemyCastle();
        else if (triggerMessage == "InEnemyRegion") trigger.InEnemyRegion();
        else if (triggerMessage == "InEnemyPalace") trigger.InEnemyPalace();
        else if (triggerMessage == "AtEnemyBottom") trigger.AtEnemyBottom();
        CancelTrigger();
    }
    public void CancelTrigger()
    {
        trigger = null;
        triggerMessage = "";
        askTriggerPanel.SetActive(false);
        showInfoCard.SetActive(false);
        if (--GameInfo.actions[InfoLoader.playerID] == 0) NextTurn();
    }
    private IEnumerator ShowFullTacticBag(float time = 1.5f)
    {
        fullTacticBag.SetActive(true);
        yield return new WaitForSeconds(time);
        fullTacticBag.SetActive(false);
    }
    public void ShowPieceFrozen()
    {
        StartCoroutine(PieceFrozen());
    }
    private IEnumerator PieceFrozen(float time = 1.5f)
    {
        freezeText.SetActive(true);
        yield return new WaitForSeconds(time);
        freezeText.SetActive(false);
    }
    public void ShowNotEnoughCoins()
    {
        StartCoroutine(NotEnoughCoins());
    }
    private IEnumerator NotEnoughCoins(float time = 1.5f)
    {
        notEnoughCoinsImage.SetActive(true);
        yield return new WaitForSeconds(time);
        notEnoughCoinsImage.SetActive(false);
    }
    public void ShowNotEnoughOres()
    {
        StartCoroutine(NotEnoughOres());
    }
    private IEnumerator NotEnoughOres(float time = 1.5f)
    {
        notEnoughOresImage.SetActive(true);
        yield return new WaitForSeconds(time);
        notEnoughOresImage.SetActive(false);
    }
    public void SetOreText()
    {
        oreText.text = GameInfo.ores[InfoLoader.playerID].ToString();
    }
    public void SetCoinText()
    {
        coinText.text = InfoLoader.user.coins.ToString();
    }
}
