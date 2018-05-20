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
    public static GameInfo gameInfo;

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

    // Used for ask trigger
    private static Trigger trigger;
    private static string triggerMessage;

    // Use this for initialization
    void Start () {
        lineup = Login.user.lineups[Login.user.lastLineupSelected];
        // Set GameInfo
        gameInfo = new GameInfo(lineup, Login.playerID, new EnemyLineup(), 99999999); // should be downloading a GameInfo
        //gameInfo.JsonToClass();
        Login.user.SetGameID(gameInfo.gameID);
        board = Instantiate(Resources.Load<GameObject>("Board/" + lineup.boardName + "/Board"));
        board.transform.SetSiblingIndex(1);
        boardSetup = board.GetComponent<BoardSetup>();
        boardSetup.Setup(lineup, Login.playerID);  // Set up Player Lineup
        boardSetup.Setup(gameInfo.lineups[gameInfo.TheOtherPlayer()], 99999999);  // Set up Enemy Lineup
        // Set up Player Info
        playerName.text = Login.user.username;
        playerWin.text = "Win%: " + Login.user.total.percentage.ToString();
        playerRank.text = "Rank: " + Login.user.rank.ToString();
        // Set up Opponent Info
        opponentName.text = "Opponent";
        opponentWin.text = "Win%: 80.00";
        opponentRank.text = "Rank: 9900";
        modeName.text = Login.user.lastModeSelected;
        foreach (var item in gameInfo.triggers)
            item.Value.StartOfGame();
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
            tacticObjs[i].GetComponent<TacticInfo>().SetAttributes(Database.FindTacticAttributes(lineup.tactics[i].tacticName));
            tacticTriggers.Add(tacticObjs[i].GetComponent<TacticInfo>().trigger);
        }
        foreach(var item in gameInfo.triggers)
        {
            if (item.Value.passive == "Tactic")
            {
                foreach (Tactic tactic in lineup.tactics)
                    if (item.Value.PassiveCriteria(tactic))
                        item.Value.Passive(tactic);
            }
            else if(item.Value.passive == "Piece")
            {
                foreach (var pair in gameInfo.board)
                    if (item.Value.PassiveCriteria(pair.Value))
                        item.Value.Passive(pair.Value);
            }
        }
        StartCoroutine(Timer());
        StartCoroutine(GameStartAnimation());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!gameInfo.gameOver) return;
        if (victoryImage.activeSelf && Login.user.winsToday <= UserInfo.maxWinPerDay)
        {
            victoryImage.SetActive(false);
            winReward.SetActive(true);
            Login.user.ChangeCoins(1);
            return;
        }
        gameInfo.gameOver = false;
        SceneManager.LoadScene("PlayerMatching");
        gameInfo.Clear();
        Destroy(board);
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
            if (gameInfo.gameOver) break;
            string seconds = (gameInfo.time % 60).ToString();
            if (seconds.Length == 1) seconds = "0" + seconds;
            timer.text = (gameInfo.time / 60).ToString() + ":" + seconds;
            if (gameInfo.time < 15) timer.color = Color.red;
            else timer.color = Color.white;
            yield return new WaitForSeconds(1.0f);
            if (--gameInfo.time < 0) NextTurn();
        }
    }

    public void Victory()
    {
        gameInfo.victory = Login.playerID;
        victoryImage.SetActive(true);
        Login.user.Win();
        GameOver();
    }
    public void Defeat()
    {
        gameInfo.victory = gameInfo.TheOtherPlayer();
        defeatImage.SetActive(true);
        Login.user.Lose();
        GameOver();
    }
    public void Draw()
    {
        bool playerTrophy = gameInfo.FindUsedTactic("Winner's Trophy", Login.playerID) != -1,
             enemyTrophy = gameInfo.FindUsedTactic("Winner's Trophy", gameInfo.TheOtherPlayer()) != -1;
        if (playerTrophy && !enemyTrophy) Victory();
        else if (!playerTrophy && enemyTrophy) Defeat();
        else
        {
            gameInfo.victory = -1;
            drawImage.SetActive(true);
            Login.user.Draw();
            GameOver();
        }
    }

    public void GameOver()
    {
        Login.user.SetGameID(-1);
        pieceInfoCard.SetActive(false);
        gameInfo.time = gameInfo.maxTime;
        gameInfo.gameOver = true;
        if (settingsPanel.activeSelf) settingsPanel.SetActive(false);
        foreach(KeyValuePair<Vector2Int,GameObject> pair in boardSetup.pieces)
        {
            Trigger trigger = pair.Value.GetComponent<PieceInfo>().trigger;
            if (trigger != null) trigger.EndOfGame();
        }
        // CalculateNewRank(); // should be done by server
        foreach (KeyValuePair<Vector2Int, Collection> pair in new Dictionary<Vector2Int, Collection>(lineup.cardLocations))
        {
            bool alive = false;
            foreach(Piece piece in gameInfo.activePieces[Login.playerID])
                if (piece.original && piece.GetCastle() == pair.Key)
                {
                    alive = true;
                    lineup.cardLocations[pair.Key] = piece.collection;
                    break;
                }
            if (!alive && pair.Value.health > 0 && --pair.Value.health == 0)
            {
                int index = Login.user.FindCollection(pair.Value);
                Login.user.RemoveCollection(index);
                int next = Login.user.FindCollection(pair.Value.name);
                if (next != -1) lineup.cardLocations[pair.Key] = Login.user.collection[next];
                else lineup.cardLocations[pair.Key] = Collection.StandardCollection(pair.Value.type);
            }
        }
        foreach(Tactic tactic in new List<Tactic>(lineup.tactics))
        {
            if (gameInfo.unusedTactics[Login.playerID].Contains(tactic)) continue;
            int index = Login.user.FindCollection(tactic.tacticName);
            if (index != -1) Login.user.ChangeCollectionCount(index, -1);
            else
            {
                lineup.tactics.Remove(tactic);
                lineup.complete = false;
            }
        }
        Login.user.ModifyLineup(lineup, Login.user.lastLineupSelected);
        //gameInfo.Upload();
    }

    public void Concede()
    {
        Defeat();
    }

    private void CalculateNewRank()
    {
        int credit = 0;
        foreach(Piece piece in gameInfo.activePieces[Login.playerID])
            credit += credits[piece.GetPieceType()];
        foreach (Piece piece in gameInfo.inactivePieces[gameInfo.TheOtherPlayer()])
            credit += credits[piece.GetPieceType()];
    }

    public void EndTurn()
    {
        endTurnButton.interactable = false;
        NextTurn();
    }

    public void YourTurn()
    {
        if (gameInfo.MultiActions(Login.playerID))
        {
            endTurnButton.interactable = true;
            endTurnText.text = "End Turn";
        }
        else
        {
            endTurnButton.interactable = false;
            endTurnText.text = "Your Turn";
        }
        //StartCoroutine(ShowYourTurn());
        gameInfo.ResetActions(Login.playerID);
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
        gameInfo.NextTurn();
        roundCount.text = gameInfo.round.ToString();
        if (gameInfo.round == 150)
        {
            Draw();
            return;
        }
        EnemyTurn();
    }

    public void EnemyTurn()
    {
        // Set Button interactable
        gameInfo.ResetActions(gameInfo.TheOtherPlayer());
        SetTacticInteractable(false);
        endTurnButton.interactable = false;
        endTurnText.text = "Enemy Turn";

        //WWWForm infoToPhp = new WWWForm(); //create WWWform to send to php script
        //infoToPhp.AddField("gameID", gameInfo.gameID);
        //infoToPhp.AddField("playerID", gameInfo.TheOtherPlayer());
        //WWW sendToPhp = new WWW("http://localhost:8888/gameinfo.php", infoToPhp);
        //while (!sendToPhp.isDone) { }
        // WWWForm
        //GameController.DecodeGameEvent(GameEvent.JsonToClass(sendToPhp.text));
        // if trigger enemyCardInfo.GetComponent<CardInfo>().SetAttributes()
        gameInfo.NextTurn();
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
        if (gameInfo.traps.ContainsKey(location))
        {
            GameEvent gameEvent = new GameEvent();
            explosion.transform.position = new Vector3(location.x * MovementController.scale, location.y * MovementController.scale, -3);
            explosion.transform.SetParent(boardSetup.boardCanvas);
            TrapAttributes trap = Database.FindTrapAttributes(gameInfo.traps[location].Key);
            trapInfoCard.GetComponent<TrapInfo>().SetAttributes(trap, gameInfo.traps[location].Value);
            trap.trigger.Activate(location);
            gameInfo.traps.Remove(location);
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
        int count = gameInfo.unusedTactics[Login.playerID].Count;
        if (count == LineupBuilder.tacticsLimit) StartCoroutine(ShowFullTacticBag());
        else
        {
            gameInfo.AddTactic(tactic);
            tacticObjs[count].parent.gameObject.SetActive(true);
            int index = 0;
            if (count != 0)
            {
                index = gameInfo.unusedTactics[Login.playerID].IndexOf(tactic);
                for (int i = count; i > index; i--)
                {
                    TacticInfo tacticInfo = tacticObjs[i - 1].GetComponent<TacticInfo>();
                    tacticObjs[i].GetComponent<TacticInfo>().SetAttributes(tacticInfo.tacticAttributes, Login.playerID, tacticInfo.tactic.original);
                }
            }
            TacticAttributes tacticAttributes = Database.FindTacticAttributes(tactic.tacticName);
            tacticObjs[index].GetComponent<TacticInfo>().SetAttributes(tacticAttributes, Login.playerID, false);
            tacticTriggers.Insert(index, tacticAttributes.trigger);
            SetTacticInteractable();
        }
    }

    public void RemoveTactic(Tactic tactic)
    {
        int index = gameInfo.FindUnusedTactic(tactic.tacticName, Login.playerID);
        int count = gameInfo.unusedTactics[Login.playerID].Count;
        if (count > 1)
        {
            for (int i = index; i < count - 1; i++)
            {
                TacticInfo tacticInfo = tacticObjs[i + 1].GetComponent<TacticInfo>();
                tacticObjs[i].GetComponent<TacticInfo>().SetAttributes(tacticInfo.tacticAttributes, Login.playerID, tacticInfo.tactic.original);
            }
        }
        else tacticObjs[0].GetComponent<TacticInfo>().Clear();
        tacticTriggers.RemoveAt(index);
        tacticObjs[count - 1].parent.gameObject.SetActive(false);
        gameInfo.RemoveTactic(index);
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
        if (trigger_para.ReceiveMesseage(message) && trigger_para.piece.oreCost <= gameInfo.ores[Login.playerID])
        {
            gameInfo.Act("ability", Login.playerID, 1);
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
        gameInfo.Act("ability", Login.playerID, -1);
        if (!gameInfo.Actable(Login.playerID)) NextTurn();
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
        oreText.text = gameInfo.ores[Login.playerID].ToString();
    }
    public void SetCoinText()
    {
        coinText.text = Login.user.coins.ToString();
    }
}
