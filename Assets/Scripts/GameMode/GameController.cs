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
        if (GameInfo.gameOver || !GameInfo.gameStarts || GameInfo.actions[InfoLoader.user.playerID] == 0 || onEnterGame.askTriggerPanel.activeSelf) return;
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Collider hitObj = hit.collider;
                if (hitObj == MovementController.selected) MovementController.PutDownPiece(); // Put down
                else if (hitObj.name == "Piece" && hitObj.GetComponent<PieceInfo>().piece.isAlly)
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
                    if (hitObj.name == "Piece") location = InfoLoader.StringToVec2(hitObj.transform.parent.name);
                    else location = InfoLoader.StringToVec2(hitObj.name);
                    if (MovementController.validLocs.Contains(location))
                    {
                        MovementController.MoveTo(location);
                        if (--GameInfo.actions[InfoLoader.playerID] == 0) onEnterGame.NextTurn();
                    }
                }
                else if (ActivateAbility.activated)
                {
                    Vector2Int location;
                    if (hitObj.name == "Piece") location = InfoLoader.StringToVec2(hitObj.transform.parent.name);
                    else location = InfoLoader.StringToVec2(hitObj.name);
                    if (ActivateAbility.targetLocs.Contains(location))
                    {
                        ActivateAbility.Activate(location);
                        if (--GameInfo.actions[InfoLoader.playerID] == 0) onEnterGame.NextTurn();
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

    public static void ChangeSide(Vector2Int location, bool isAlly)
    {
        boardSetup.pieces[location].GetComponent<PieceInfo>().piece.isAlly = isAlly;
        Piece piece = GameInfo.board[location];
        if (isAlly)
        {
            GameInfo.activePieces[GameInfo.TheOtherPlayer()].Remove(piece);
            piece.isAlly = isAlly;
            GameInfo.activePieces[InfoLoader.playerID].Add(piece);
        }
        else
        {
            GameInfo.activePieces[InfoLoader.playerID].Remove(piece);
            piece.isAlly = isAlly;
            GameInfo.activePieces[GameInfo.TheOtherPlayer()].Add(piece);
        }
        // upload
    }

    public static void AddPiece(Collection collection, Vector2Int castle, bool isAlly)
    {
        boardSetup.AddPiece(collection, castle, isAlly, false);
    }

    public static void ResurrectPiece(Collection collection, Vector2Int castle, bool isAlly)
    {
        boardSetup.AddPiece(collection, castle, isAlly, true, true);
    }

    public static void AddTactic(Tactic tactic)
    {
        onEnterGame.AddTactic(tactic);
    }

    public static void RemoveTactic(Tactic tactic)
    {
        onEnterGame.RemoveTactic(tactic);
    }

    public static void ChangePieceHealth(Vector2Int location, int deltaAmount)
    {
        Piece before = GameInfo.board[location];
        Piece after = new Piece(before);
        after.health += deltaAmount;
        after.collection.health += deltaAmount;
        GameInfo.board[location] = after;
        if (before.isAlly) GameInfo.activePieces[InfoLoader.playerID][GameInfo.activePieces[InfoLoader.playerID].IndexOf(before)] = after;
        else GameInfo.activePieces[GameInfo.TheOtherPlayer()][GameInfo.activePieces[GameInfo.TheOtherPlayer()].IndexOf(before)] = after;
        // upload
    }

    public static void ChangePieceOreCost(Vector2Int location, int deltaAmount)
    {
        Piece before = GameInfo.board[location];
        Piece after = new Piece(before);
        after.oreCost += deltaAmount;
        GameInfo.board[location] = after;
        if (before.isAlly) GameInfo.activePieces[InfoLoader.playerID][GameInfo.activePieces[InfoLoader.playerID].IndexOf(before)] = after;
        else GameInfo.activePieces[GameInfo.TheOtherPlayer()][GameInfo.activePieces[GameInfo.TheOtherPlayer()].IndexOf(before)] = after;
        // upload
    }

    public static void ChangeTacticOreCost(string tacticName, int deltaAmount)
    {
        int index = GameInfo.FindTactic(tacticName, InfoLoader.playerID);
        Tactic tactic = GameInfo.unusedTactics[InfoLoader.playerID][index];
        tactic.oreCost += deltaAmount;
        GameInfo.unusedTactics[InfoLoader.playerID][index] = tactic;
        onEnterGame.ChangeTacticOreCost(index, deltaAmount);
        // upload
    }

    public static void Eliminate(Piece piece)
    {
        Destroy(boardSetup.pieces[piece.location]);
        boardSetup.pieces.Remove(piece.location);
        GameInfo.RemovePiece(piece);
        // upload
    }

    public static void Eliminate(Vector2Int location)
    {
        Destroy(boardSetup.pieces[location]);
        boardSetup.pieces.Remove(location);
        GameInfo.RemovePiece(GameInfo.board[location]);
        // upload
    }

    public static void FreezePiece(Vector2Int location, int round)
    {
        Piece piece = GameInfo.board[location];
        int index = GameInfo.activePieces[InfoLoader.playerID].IndexOf(piece);
        if (index == -1)
        {
            index = GameInfo.activePieces[GameInfo.TheOtherPlayer()].IndexOf(piece);
            GameInfo.activePieces[GameInfo.TheOtherPlayer()][index].freeze = round;
        }
        else GameInfo.activePieces[InfoLoader.playerID][index].freeze = round;
        GameInfo.board[location].freeze = round;
        boardSetup.pieces[location].GetComponent<PieceInfo>().piece.freeze = round;

        // Add freeze image
        GameObject freezeImage = Instantiate(onEnterGame.freezeImage, boardCanvas);
        freezeImage.transform.position = new Vector3(location.x * MovementController.scale, location.y * MovementController.scale, -0.5f);
        freezeImages.Add(location, freezeImage);
        // upload
    }

    public static void PlaceTrap(Vector2Int location, string trapName, int creator)
    {
        GameInfo.traps.Add(location, new KeyValuePair<string, int>(trapName, creator));
        // upload
    }

    public static void PlaceFlag(Vector2Int location, bool isAlly)
    {
        GameObject flag;
        if (isAlly)
        {
            flag = Instantiate(onEnterGame.playerFlag, boardCanvas);
            GameInfo.flags.Add(location, InfoLoader.playerID);
        }
        else
        {
            flag = Instantiate(onEnterGame.enemyFlag, boardCanvas);
            GameInfo.flags.Add(location, GameInfo.TheOtherPlayer());
        }
        flag.transform.position = new Vector3(location.x * MovementController.scale, location.y * MovementController.scale, -0.5f);
        flags.Add(location, flag);
        // upload
    }

    public static void RemoveFlag(Vector2Int location)
    {
        Destroy(flags[location]);
        flags.Remove(location);
        GameInfo.flags.Remove(location);
        // upload
    }

    public static void RemoveTrap(Vector2Int location)
    {
        GameInfo.traps.Remove(location);
        // upload
    }

    public static void DecodeGameEvent(GameEvent gameEvent)
    {
        if (gameEvent.move)
        {

        }
        else if (gameEvent.trap)
        {

        }
        else if (gameEvent.piece)
        {

        }
        else if (gameEvent.tactic)
        {

        }
    }

    public static bool ChangeOre(int deltaAmount)
    {
        if(GameInfo.ores[InfoLoader.playerID] + deltaAmount < 0)
        {
            onEnterGame.ShowNotEnoughOres();
            return false;
        }
        GameInfo.ores[InfoLoader.playerID] += deltaAmount;
        onEnterGame.SetOreText();
        return true;
    }
    public static bool ChangeCoin(int deltaAmount)
    {
        if(InfoLoader.user.coins + deltaAmount < 0)
        {
            onEnterGame.ShowNotEnoughCoins();
            return false;
        }
        InfoLoader.user.ChangeCoins(deltaAmount);
        onEnterGame.SetCoinText();
        return true;
    }
    public static List<Vector2Int> FindCastles(string type) { return castles[type]; }
}
