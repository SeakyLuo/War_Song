using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static float raiseHeight = -3f;
    public static Vector3 raiseVector = new Vector3(0, 0, raiseHeight);
    public static OnEnterGame onEnterGame;
    public static BoardSetup boardSetup;
    public static Transform boardCanvas;
    public static Dictionary<string, List<Vector2Int>> castles;
    public static PieceInfo pieceInfo;
    public static Dictionary<Vector2Int, GameObject> flags;
    public static Dictionary<Vector2Int, GameObject> freezeImages;

    [HideInInspector] public GameObject settingsPanel;

    // Use this for initialization
    void Start () {
        onEnterGame = GameObject.Find("UIPanel").GetComponent<OnEnterGame>();
        boardSetup = onEnterGame.boardSetup;
        boardCanvas = onEnterGame.board.transform.Find("Canvas");
        settingsPanel = onEnterGame.settingsPanel;
        castles = new Dictionary<string, List<Vector2Int>>()
        {
            {"Advisor", boardSetup.boardAttributes.AdvisorCastle() },
            {"Elephant", boardSetup.boardAttributes.ElephantCastle()  },
            {"Horse", boardSetup.boardAttributes.HorseCastle()  },
            {"Chariot", boardSetup.boardAttributes.ChariotCastle()  },
            {"Cannon", boardSetup.boardAttributes.CannonCastle()  },
            {"Soldier", boardSetup.boardAttributes.SoldierCastle()  },
        };
        flags = new Dictionary<Vector2Int, GameObject>();
        freezeImages = new Dictionary<Vector2Int, GameObject>();
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
                else if (MovementController.selected != null && !ActivateAbility.activated)
                {
                    Vector2Int location;
                    if (hitObj.name == "Piece") location = Database.StringToVec2(hitObj.transform.parent.name);
                    else location = Database.StringToVec2(hitObj.name);
                    if (MovementController.validLocs.Contains(location))
                    {
                        OnEnterGame.gameInfo.Act("move", Login.playerID);
                        MovementController.MoveTo(location);
                        if (!OnEnterGame.gameInfo.Actable(Login.playerID)) onEnterGame.NextTurn();
                    }
                }
                else if (ActivateAbility.activated)
                {
                    Vector2Int location;
                    if (hitObj.name == "Piece") location = Database.StringToVec2(hitObj.transform.parent.name);
                    else location = Database.StringToVec2(hitObj.name);
                    if (ActivateAbility.targetLocs.Contains(location))
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

    public static void ChangeSide(Vector2Int location, int ownerID)
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

    public static void AddPiece(Collection collection, Vector2Int castle, int ownerID)
    {
        boardSetup.AddPiece(collection, castle, ownerID, false);
        Passive(OnEnterGame.gameInfo.board[castle], ownerID);
    }

    public static void ResurrectPiece(Collection collection, Vector2Int castle, int ownerID)
    {
        boardSetup.AddPiece(collection, castle, ownerID, true, true);
        Passive(OnEnterGame.gameInfo.board[castle], ownerID);
    }

    public static void AddTactic(Tactic tactic)
    {
        onEnterGame.AddTactic(tactic);
        Passive(tactic, Login.playerID);
    }

    public static void RemoveTactic(Tactic tactic)
    {
        onEnterGame.RemoveTactic(tactic);
    }

    public static void ChangePieceHealth(Vector2Int location, int deltaAmount)
    {
        Piece before = OnEnterGame.gameInfo.board[location];
        Piece after = new Piece(before);
        after.health += deltaAmount;
        after.collection.health += deltaAmount;
        OnEnterGame.gameInfo.board[location] = after;
        OnEnterGame.gameInfo.triggers[location].piece = after;
        if (before.IsAlly()) OnEnterGame.gameInfo.activePieces[Login.playerID][OnEnterGame.gameInfo.activePieces[Login.playerID].IndexOf(before)] = after;
        else OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()][OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()].IndexOf(before)] = after;
        OnEnterGame.gameInfo.Upload();
    }
    public static void ChangePieceOreCost(Vector2Int location, int deltaAmount)
    {
        Piece before = OnEnterGame.gameInfo.board[location];
        Piece after = new Piece(before);
        after.oreCost += deltaAmount;
        OnEnterGame.gameInfo.board[location] = after;
        OnEnterGame.gameInfo.triggers[location].piece = after;
        if (before.IsAlly()) OnEnterGame.gameInfo.activePieces[Login.playerID][OnEnterGame.gameInfo.activePieces[Login.playerID].IndexOf(before)] = after;
        else OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()][OnEnterGame.gameInfo.activePieces[OnEnterGame.gameInfo.TheOtherPlayer()].IndexOf(before)] = after;
        OnEnterGame.gameInfo.Upload();
    }
    public static void ChangeTacticOreCost(string tacticName, int deltaAmount)
    {
        int index = OnEnterGame.gameInfo.FindUnusedTactic(tacticName, Login.playerID);
        Tactic tactic = OnEnterGame.gameInfo.unusedTactics[Login.playerID][index];
        tactic.oreCost += deltaAmount;
        OnEnterGame.gameInfo.unusedTactics[Login.playerID][index] = tactic;
        onEnterGame.ChangeTacticOreCost(index, deltaAmount);
        OnEnterGame.gameInfo.Upload();
    }
    public static void ChangeTacticCoinCost(string tacticName, int deltaAmount)
    {
        int index = OnEnterGame.gameInfo.FindUnusedTactic(tacticName, Login.playerID);
        Tactic tactic = OnEnterGame.gameInfo.unusedTactics[Login.playerID][index];
        tactic.oreCost += deltaAmount;
        OnEnterGame.gameInfo.unusedTactics[Login.playerID][index] = tactic;
        onEnterGame.ChangeTacticGoldCost(index, deltaAmount);
        OnEnterGame.gameInfo.Upload();
    }

    public static void Eliminate(Piece piece, bool revenge = true)
    {
        GameEvent gameEvent;
        if (revenge)
        {
            OnEnterGame.gameInfo.triggers[piece.location].Revenge();
            gameEvent = new GameEvent(piece);
            onEnterGame.AddToHistory(gameEvent);
        }
        gameEvent = new GameEvent(piece, "Kill");
        onEnterGame.AddToHistory(gameEvent);
        Destroy(boardSetup.pieces[piece.location]);
        boardSetup.pieces.Remove(piece.location);
        OnEnterGame.gameInfo.RemovePiece(piece);
    }

    public static void Eliminate(Vector2Int location, bool revenge = true)
    {
        if (revenge)
        {
            OnEnterGame.gameInfo.triggers[location].Revenge();
            GameEvent gameEvent = new GameEvent(OnEnterGame.gameInfo.board[location]);
            onEnterGame.AddToHistory(gameEvent);
        }
        Destroy(boardSetup.pieces[location]);
        boardSetup.pieces.Remove(location);
        OnEnterGame.gameInfo.RemovePiece(OnEnterGame.gameInfo.board[location]);
        OnEnterGame.gameInfo.Upload();
    }

    public static void FreezePiece(Vector2Int location, int round)
    {
        GameEvent gameEvent = new GameEvent(OnEnterGame.gameInfo.board[location], "freeze");
        OnEnterGame.gameInfo.FreezePiece(location, round);
        boardSetup.pieces[location].GetComponent<PieceInfo>().piece.freeze = round;

        // Add freeze image
        GameObject freezeImage = Instantiate(onEnterGame.freezeImage, boardCanvas);
        freezeImage.transform.position = new Vector3(location.x * MovementController.scale, location.y * MovementController.scale, -0.5f);
        freezeImages.Add(location, freezeImage);
    }

    public static void PlaceTrap(Vector2Int location, string trapName, int creator)
    {
        OnEnterGame.gameInfo.traps.Add(location, new KeyValuePair<string, int>(trapName, creator));
        OnEnterGame.gameInfo.Upload();
    }

    public static void PlaceFlag(Vector2Int location, bool isAlly)
    {
        GameObject flag;
        if (isAlly)
        {
            flag = Instantiate(onEnterGame.playerFlag, boardCanvas);
            OnEnterGame.gameInfo.flags.Add(location, Login.playerID);
        }
        else
        {
            flag = Instantiate(onEnterGame.enemyFlag, boardCanvas);
            OnEnterGame.gameInfo.flags.Add(location, OnEnterGame.gameInfo.TheOtherPlayer());
        }
        flag.transform.position = new Vector3(location.x * MovementController.scale, location.y * MovementController.scale, -0.5f);
        flags.Add(location, flag);
        OnEnterGame.gameInfo.Upload();
    }

    public static void RemoveFlag(Vector2Int location)
    {
        Destroy(flags[location]);
        flags.Remove(location);
        OnEnterGame.gameInfo.flags.Remove(location);
        OnEnterGame.gameInfo.Upload();
    }

    public static void RemoveTrap(Vector2Int location)
    {
        OnEnterGame.gameInfo.traps.Remove(location);
        OnEnterGame.gameInfo.Upload();
    }

    public static void DecodeGameEvent(GameEvent gameEvent)
    {
        if (gameEvent.result == "move")
        {
            MovementController.Move(OnEnterGame.gameInfo.board[gameEvent.eventLocation], gameEvent.eventLocation, gameEvent.targetLocation);
        }
        else
        {

        }
    }

    public static void Passive(Piece piece, int caller)
    {
        foreach (Piece activePiece in OnEnterGame.gameInfo.activePieces[caller])
        {
            Trigger trigger = OnEnterGame.gameInfo.triggers[activePiece.location];
            if (trigger.passive == "Piece" && trigger.PassiveCriteria(piece)) trigger.Passive(piece);
        }
    }

    public static void ResumePiece(Piece piece)
    {
        piece.oreCost = piece.collection.oreCost;
        piece.health = piece.collection.health;
        piece.freeze = 0;
    }

    public static void ResumeTactic(Tactic tactic)
    {
        TacticAttributes tacticAttributes = Database.FindTacticAttributes(tactic.tacticName);
        tactic.oreCost = tacticAttributes.oreCost;
        tactic.goldCost = tacticAttributes.goldCost;
    }

    public static void Passive(Tactic tactic, int caller)
    {   
        foreach (Piece piece in OnEnterGame.gameInfo.activePieces[caller])
        {
            Trigger trigger = OnEnterGame.gameInfo.triggers[piece.location];
            if (trigger.passive == "Tactic" && trigger.PassiveCriteria(tactic)) trigger.Passive(tactic);
        }
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
    public static List<Vector2Int> FindCastles(string type) { return castles[type]; }
}
