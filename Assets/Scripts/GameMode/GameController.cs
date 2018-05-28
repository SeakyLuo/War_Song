using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static float raiseHeight = -3f;
    public static Vector3 raiseVector = new Vector3(0, 0, raiseHeight);
    public static OnEnterGame onEnterGame;
    public static BoardSetup boardSetup;
    public static Transform boardCanvas;
    public static Dictionary<string, List<Location>> castles;
    public static PieceInfo pieceInfo;
    public static Dictionary<Location, GameObject> flags;
    public static Dictionary<Location, GameObject> freezeImages;

    [HideInInspector] public GameObject settingsPanel;

    // Use this for initialization
    void Start () {
        onEnterGame = GameObject.Find("UIPanel").GetComponent<OnEnterGame>();
        boardSetup = onEnterGame.boardSetup;
        boardCanvas = onEnterGame.board.transform.Find("Canvas");
        settingsPanel = onEnterGame.settingsPanel;
        castles = new Dictionary<string, List<Location>>()
        {
            {"Advisor", boardSetup.boardAttributes.AdvisorCastle() },
            {"Elephant", boardSetup.boardAttributes.ElephantCastle()  },
            {"Horse", boardSetup.boardAttributes.HorseCastle()  },
            {"Chariot", boardSetup.boardAttributes.ChariotCastle()  },
            {"Cannon", boardSetup.boardAttributes.CannonCastle()  },
            {"Soldier", boardSetup.boardAttributes.SoldierCastle()  },
        };
        flags = new Dictionary<Location, GameObject>();
        freezeImages = new Dictionary<Location, GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            settingsPanel.SetActive(true);
            MovementController.PutDownPiece();
        }
        //Debug.Log(OnEnterGame.gameInfo.gameOver.ToString() + "   " +(!OnEnterGame.gameInfo.gameStarts).ToString() + "   " + (OnEnterGame.gameInfo.currentTurn == Login.playerID).ToString() + "   " + onEnterGame.askTriggerPanel.activeSelf.ToString());
        if (OnEnterGame.gameInfo.gameOver ||
            !OnEnterGame.gameInfo.gameStarts ||
            OnEnterGame.gameInfo.currentTurn == Login.playerID ||
            onEnterGame.askTriggerPanel.activeSelf) return;
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Collider hitObj = hit.collider;
                if (hitObj == MovementController.selected) MovementController.PutDownPiece(); // Put down
                else if (hitObj.name == "Piece" && hitObj.GetComponent<PieceInfo>().piece.IsAlly())
                {
                    pieceInfo = hitObj.GetComponent<PieceInfo>();
                    if(pieceInfo.piece.freeze > 0)
                    {
                        onEnterGame.ShowPieceFrozen();
                        return;
                    }
                    if (ActivateAbility.activated) GameTacticGesture.Resume();
                    else if (MovementController.selected != null) MovementController.PutDownPiece(); // switch piece
                    hit.collider.transform.position += raiseVector;
                    MovementController.selected = hit.collider;
                    pieceInfo = hitObj.GetComponent<PieceInfo>();
                    MovementController.pieceInfo = ActivateAbility.pieceInfo = pieceInfo;
                    pieceInfo.HideInfoCard();
                    ActivateAbility.ActivateButton();
                    MovementController.validLocs = pieceInfo.ValidLoc();
                    MovementController.DrawPathDots();
                }
                else
                {
                    if (hitObj.name == "UIPanel") return;
                    Location location;
                    if (hitObj.name == "Piece") location = new Location(hitObj.transform.parent.name);
                    else
                    {
                        if (!Location.CorrectFormat(hitObj.name)) return;
                        location = new Location(hitObj.name);
                    }
                    if (MovementController.selected != null && !ActivateAbility.activated)
                    {
                        OnEnterGame.gameInfo.Act("move", Login.playerID);
                        MovementController.MoveTo(location);
                        if (!OnEnterGame.gameInfo.Actable(Login.playerID)) onEnterGame.NextTurn();
                    }
                    else if (ActivateAbility.activated)
                    {
                        OnEnterGame.gameInfo.Act(ActivateAbility.actor, Login.playerID);
                        ActivateAbility.Activate(location);
                        if (!OnEnterGame.gameInfo.Actable(Login.playerID)) onEnterGame.NextTurn();
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (MovementController.selected != null)
            {
                MovementController.PutDownPiece();
                if (ActivateAbility.activated) ActivateAbility.RemoveTargets();
            }
            else if (OnEnterGame.current_tactic != -1)
            {
                GameTacticGesture.Resume();
            }
        }
    }

    public static void ChangeSide(Location location, int ownerID)
    {
        boardSetup.pieces[location].GetComponent<PieceInfo>().piece.ownerID = ownerID;
        Piece piece = OnEnterGame.gameInfo.board[location];
        if (ownerID == Login.playerID)
        {
            OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()].Remove(piece);
            piece.ownerID = ownerID;
            OnEnterGame.gameInfo.activePieces[Login.playerID].Add(piece);
        }
        else
        {
            OnEnterGame.gameInfo.activePieces[Login.playerID].Remove(piece);
            piece.ownerID = ownerID;
            OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()].Add(piece);
        }
        OnEnterGame.gameInfo.triggers[location].piece = piece;
        Passive(OnEnterGame.gameInfo.board[location], ownerID);
        OnEnterGame.gameInfo.Upload();
    }

    public static void AddPiece(Collection collection, Location castle, int ownerID)
    {
        boardSetup.AddPiece(collection, castle, ownerID, false);
        Passive(OnEnterGame.gameInfo.board[castle], ownerID);
    }

    public static void ResurrectPiece(Collection collection, Location castle, int ownerID)
    {
        boardSetup.AddPiece(collection, castle, ownerID, true, true);
        Passive(OnEnterGame.gameInfo.board[castle], ownerID);
    }

    public static void AddTactic(Tactic tactic)
    {
        onEnterGame.AddTactic(tactic);
        Passive(tactic, Login.playerID);
    }

    public static void RemoveTactic(Tactic tactic, bool useTactic = false)
    {
        onEnterGame.RemoveTactic(tactic);
        if(!useTactic) onEnterGame.AddToHistory(new GameEvent("Discard", tactic));
    }

    public static void ChangePieceHealth(Location location, int deltaAmount, GameEvent gameEvent = null)
    {
        Piece before = OnEnterGame.gameInfo.board[location];
        Piece after = new Piece(before);
        after.health += deltaAmount;
        after.collection.health += deltaAmount;
        OnEnterGame.gameInfo.board[location] = after;
        OnEnterGame.gameInfo.triggers[location].piece = after;
        boardSetup.pieces[location].GetComponent<PieceInfo>().SetPiece(after);
        if (before.IsAlly()) OnEnterGame.gameInfo.activePieces[Login.playerID][OnEnterGame.gameInfo.activePieces[Login.playerID].IndexOf(before)] = after;
        else OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()][OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()].IndexOf(before)] = after;
        OnEnterGame.gameInfo.Upload();
        if (gameEvent == null) gameEvent = new GameEvent("PieceHealth", before, after, deltaAmount);
        onEnterGame.AddToHistory(gameEvent);
    }
    public static void ChangePieceOreCost(Location location, int deltaAmount, GameEvent gameEvent = null)
    {
        Piece before = OnEnterGame.gameInfo.board[location];
        Piece after = new Piece(before);
        after.oreCost += deltaAmount;
        OnEnterGame.gameInfo.board[location] = after;
        OnEnterGame.gameInfo.triggers[location].piece = after;
        boardSetup.pieces[location].GetComponent<PieceInfo>().SetPiece(after);
        if (before.IsAlly()) OnEnterGame.gameInfo.activePieces[Login.playerID][OnEnterGame.gameInfo.activePieces[Login.playerID].IndexOf(before)] = after;
        else OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()][OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()].IndexOf(before)] = after;
        OnEnterGame.gameInfo.Upload();
        if (gameEvent == null) gameEvent = new GameEvent("PieceCost", before, after, deltaAmount);
        onEnterGame.AddToHistory(gameEvent);
    }
    public static void ChangeTacticOreCost(string tacticName, int deltaAmount, GameEvent gameEvent = null)
    {
        int index = OnEnterGame.gameInfo.FindUnusedTactic(tacticName, Login.playerID);
        Tactic tactic = OnEnterGame.gameInfo.unusedTactics[Login.playerID][index];
        tactic.oreCost += deltaAmount;
        OnEnterGame.gameInfo.unusedTactics[Login.playerID][index] = tactic;
        onEnterGame.ChangeTacticOreCost(index, deltaAmount);
        OnEnterGame.gameInfo.Upload();
        if (gameEvent == null) gameEvent = new GameEvent("TacticOre", tactic, deltaAmount);
        onEnterGame.AddToHistory(gameEvent);
    }
    public static void ChangeTacticGoldCost(string tacticName, int deltaAmount, GameEvent gameEvent = null)
    {
        int index = OnEnterGame.gameInfo.FindUnusedTactic(tacticName, Login.playerID);
        Tactic tactic = OnEnterGame.gameInfo.unusedTactics[Login.playerID][index];
        tactic.oreCost += deltaAmount;
        OnEnterGame.gameInfo.unusedTactics[Login.playerID][index] = tactic;
        onEnterGame.ChangeTacticGoldCost(index, deltaAmount);
        OnEnterGame.gameInfo.Upload();
        if (gameEvent == null) gameEvent = new GameEvent("TacticGold", tactic, deltaAmount);
        onEnterGame.AddToHistory(gameEvent);
    }

    public static void Eliminate(Piece piece, bool revenge = true, GameEvent gameEvent = null)
    {
        if (revenge)
        {
            OnEnterGame.gameInfo.triggers[piece.location].Revenge();
            onEnterGame.AddToHistory(new GameEvent(piece));
        }
        gameEvent = new GameEvent(piece, "Kill");
        onEnterGame.AddToHistory(gameEvent);
        Destroy(boardSetup.pieces[piece.location]);
        boardSetup.pieces.Remove(piece.location);
        OnEnterGame.gameInfo.RemovePiece(piece);

        if (gameEvent == null) gameEvent = new GameEvent(gameEvent.eventLocation, gameEvent.eventPlayerID);
        onEnterGame.AddToHistory(gameEvent);
    }

    public static void Eliminate(Location location, Piece triggeredByPiece = null, bool revenge = true, GameEvent gameEvent = null)
    {
        if (revenge)
        {
            OnEnterGame.gameInfo.triggers[location].Revenge();
            onEnterGame.AddToHistory(new GameEvent(OnEnterGame.gameInfo.board[location]));
        }
        Destroy(boardSetup.pieces[location]);
        boardSetup.pieces.Remove(location);
        OnEnterGame.gameInfo.RemovePiece(OnEnterGame.gameInfo.board[location]);

        if (gameEvent == null)
        {
            if (triggeredByPiece == null) gameEvent = new GameEvent(triggeredByPiece, "Kill");
            else gameEvent = new GameEvent(triggeredByPiece, "Kill");
        }
        onEnterGame.AddToHistory(gameEvent);
    }

    public static void TransformPiece(Piece from, Piece into, GameEvent gameEvent = null)
    {
        boardSetup.TransformPiece(from.location, into);
        onEnterGame.Defreeze(from.location);

        if (gameEvent == null) gameEvent = new GameEvent("Transform", from, into);
        onEnterGame.AddToHistory(gameEvent);
    }

    public static void FreezePiece(Location location, int round, GameEvent gameEvent = null)
    {
        OnEnterGame.gameInfo.FreezePiece(location, round);
        boardSetup.pieces[location].GetComponent<PieceInfo>().piece.freeze = round;

        // Add freeze image
        GameObject freezeImage = Instantiate(onEnterGame.freezeImage, boardCanvas);
        freezeImage.transform.position = new Vector3(location.x * MovementController.scale, location.y * MovementController.scale, -0.5f);
        freezeImages.Add(location, freezeImage);

        if (gameEvent == null) gameEvent = new GameEvent("Freeze", OnEnterGame.gameInfo.board[location], round);
        onEnterGame.AddToHistory(gameEvent);
    }

    public static void PlaceTrap(Location location, string trapName, int creator)
    {
        OnEnterGame.gameInfo.traps.Add(location, new KeyValuePair<string, int>(trapName, creator));
        OnEnterGame.gameInfo.Upload();
    }

    public static void PlaceFlag(Location location, int ownerID, GameEvent gameEvent = null)
    {
        GameObject flag;
        if (ownerID == Login.playerID) flag = Instantiate(onEnterGame.playerFlag, boardCanvas);
        else flag = Instantiate(onEnterGame.enemyFlag, boardCanvas);
        OnEnterGame.gameInfo.flags.Add(location, Login.playerID);
        flag.transform.position = new Vector3(location.x * MovementController.scale, location.y * MovementController.scale, -0.5f);
        flags.Add(location, flag);
        OnEnterGame.gameInfo.Upload();

        if (gameEvent == null) gameEvent = new GameEvent(location, ownerID);
        onEnterGame.AddToHistory(gameEvent);
    }

    public static void RemoveFlag(Location location, GameEvent gameEvent = null)
    {
        Destroy(flags[location]);
        flags.Remove(location);
        OnEnterGame.gameInfo.flags.Remove(location);
        OnEnterGame.gameInfo.Upload();

        if (gameEvent == null) gameEvent = new GameEvent(location);
        onEnterGame.AddToHistory(gameEvent);
    }

    public static void RemoveTrap(Location location)
    {
        OnEnterGame.gameInfo.traps.Remove(location);
        OnEnterGame.gameInfo.Upload();
    }

    public static void Passive(Piece piece, int caller)
    {
        foreach (Piece activePiece in OnEnterGame.gameInfo.activePieces[caller])
        {
            Trigger trigger = OnEnterGame.gameInfo.triggers[activePiece.location];
            if (trigger.passive == "Piece" && trigger.PassiveCriteria(piece)) trigger.Passive(piece);
        }
    }

    public static void Passive(Tactic tactic, int caller)
    {   
        foreach (Piece piece in OnEnterGame.gameInfo.activePieces[caller])
        {
            Trigger trigger = OnEnterGame.gameInfo.triggers[piece.location];
            if (trigger.passive == "Tactic" && trigger.PassiveCriteria(tactic)) trigger.Passive(tactic);
        }
    }

    public static void ResumePiece(Piece piece)
    {
        PieceAttributes attributes = Database.FindPieceAttributes(piece.GetName());
        piece.oreCost = attributes.oreCost;
        piece.health = piece.collection.health;
        piece.freeze = 0;
    }

    public static void ResumeTactic(Tactic tactic)
    {
        TacticAttributes attributes = Database.FindTacticAttributes(tactic.tacticName);
        tactic.oreCost = attributes.oreCost;
        tactic.goldCost = attributes.goldCost;
    }

    public static bool ChangeOre(int deltaAmount)
    {
        if(OnEnterGame.gameInfo.ores[Login.playerID] + deltaAmount < 0)
        {
            onEnterGame.ShowNotEnoughOres();
            return false;
        }
        OnEnterGame.gameInfo.ores[Login.playerID] += deltaAmount;
        onEnterGame.SetOreText();
        return true;
    }
    public static bool ChangeCoin(int deltaAmount)
    {
        if(Login.user.coins + deltaAmount < 0)
        {
            onEnterGame.ShowNotEnoughCoins();
            return false;
        }
        Login.user.ChangeCoins(deltaAmount);
        onEnterGame.SetCoinText();
        return true;
    }
    public static List<Location> FindCastles(string type) { return castles[type]; }

    public static void DecodeGameEvent(GameEvent gameEvent)
    {
        switch (gameEvent.result)
        {
            case "Move":
                MovementController.Move(OnEnterGame.gameInfo.board[gameEvent.eventLocation], gameEvent.eventLocation, gameEvent.targetLocation);
                break;
            case "Kill":
                Eliminate(OnEnterGame.gameInfo.board[gameEvent.targetLocation]);
                break;
            case "Freeze":
                FreezePiece(gameEvent.targetLocation, gameEvent.amount);
                break;
            case "Flag":
                PlaceFlag(gameEvent.eventLocation, gameEvent.eventPlayerID, gameEvent);
                break;
            case "RemoveFlag":
                RemoveFlag(gameEvent.eventLocation);
                break;
            case "Trap":
                onEnterGame.TriggerTrap(gameEvent.eventLocation);
                break;
            case "PieceCost":
                ChangePieceOreCost(gameEvent.eventLocation, gameEvent.amount, gameEvent);
                break;
            case "PieceHealth":
                ChangePieceHealth(gameEvent.eventLocation, gameEvent.amount, gameEvent);
                break;
            case "TacticGold":
                ChangeTacticGoldCost(gameEvent.targetTriggerName, gameEvent.amount, gameEvent);
                break;
            case "TacticOre":
                ChangeTacticOreCost(gameEvent.targetTriggerName, gameEvent.amount, gameEvent);
                break;
            case "Discard":
                RemoveTactic(new Tactic(Database.FindTacticAttributes(gameEvent.targetTriggerName)), false);
                break;
        }
    }
}
