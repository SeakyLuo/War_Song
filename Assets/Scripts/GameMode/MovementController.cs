using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementController : MonoBehaviour
{
    public static float raiseHeight = -3f;
    public static Vector3 raiseVector = new Vector3(0, 0, raiseHeight);
    public static BoardAttributes boardAttributes;
    public static List<Vector2Int> validLoc = new List<Vector2Int>();
    public static Collider selected;
    public static PieceInfo pieceInfo;
    public static float scale;
    public static Transform boardCanvas;
    public static float speed = 1f;

    public Sprite newLocation;    
    
    private static GameObject oldLocation;
    private static GameObject pathDot;
    private static List<GameObject> pathDots = new List<GameObject>();
    private static Image previousImage;
    private static Sprite previousSprite;
    private static BoardSetup boardSetup;
    private static OnEnterGame onEnterGame;

    private void Start()
    {
        GameObject UIPanel = GameObject.Find("UIPanel");
        boardCanvas = transform.Find("Canvas");
        onEnterGame = UIPanel.GetComponent<OnEnterGame>();
        scale = transform.localScale.x;
        boardSetup = GetComponent<BoardSetup>();
        boardAttributes = boardSetup.boardAttributes;
        oldLocation = Instantiate(onEnterGame.oldLocation);
        oldLocation.transform.position = new Vector3(0, 0, 100);
        pathDot = onEnterGame.pathDot;
    }

    private void Update()
    {
        if (OnEnterGame.gameover || GameInfo.actionTaken || ActivateAbility.activated) return;
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Collider hitObj = hit.collider;
                if (hitObj == selected) PutDownPiece(); // Put down
                else if (hitObj.tag == "Ally")
                {
                    if (selected != null) PutDownPiece(); // switch piece
                    hit.collider.transform.position += raiseVector;
                    selected = hit.collider;
                    pieceInfo = selected.GetComponent<PieceInfo>();
                    pieceInfo.HideInfoCard();
                    ActivateAbility.ActivateButton();
                    validLoc = pieceInfo.ValidLoc();
                    if (validLoc.Count == 0) return;
                    // Draw Valid path
                    foreach (Vector2Int path in validLoc)
                    {
                        float posZ = -1;
                        if (FindAt(path) == 'E') posZ -= hitObj.transform.localScale.z;
                        GameObject copy = Instantiate(pathDot);
                        copy.name = InfoLoader.Vec2ToString(path);
                        copy.transform.position = new Vector3(path.x * scale, path.y * scale, posZ);
                        if (oldLocation.transform.position == copy.transform.position) oldLocation.SetActive(false);
                        pathDots.Add(copy);
                    }
                }
                else if (selected != null)
                {
                    Vector2Int location;
                    if (hitObj.name == "Piece") location = InfoLoader.StringToVec2(hitObj.transform.parent.name);
                    else location = InfoLoader.StringToVec2(hitObj.name);
                    if (validLoc.Contains(location))
                    {
                        KillAt(location);
                        SetLocation(location);
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(1) && selected != null) PutDownPiece();
    }

    public static void KillAt(Vector2Int loc)
    {
        Piece enemy;
        if (GameInfo.board.TryGetValue(loc, out enemy) && !enemy.isAlly)
        {
            pieceInfo.trigger.Bloodthirsty();
            GameController.Eliminate(enemy);
            if (enemy.GetPieceType() == "General")
                onEnterGame.Victory();
        }
    }

    private static Vector2Int GetGridLocation(float x, float y) { return new Vector2Int((int)Mathf.Floor(x / scale), (int)Mathf.Floor(y / scale)); }

    public static void PutDownPiece()
    {
        if (selected == null) return;
        HidePathDots();
        selected.transform.position -= raiseVector;
        if (!oldLocation.activeSelf && previousSprite != null) oldLocation.SetActive(true);
        ActivateAbility.DeactivateButton();
        pieceInfo.selected = true;
        selected = null;
    }

    public static void HidePathDots()
    {
        foreach (GameObject pathDot in pathDots) Destroy(pathDot);
        pathDots.Clear();
    }

    private void SetLocation(Vector2Int location)
    {
        /// location is new location
        oldLocation.transform.position = selected.transform.position - raiseVector;
        if (previousSprite != null)
            previousImage.sprite = previousSprite;

        Move(selected.gameObject, GetGridLocation(selected.transform.position.x, selected.transform.position.y), location);

        previousImage = selected.transform.parent.Find("Image").GetComponent<Image>();
        previousSprite = previousImage.sprite;
        previousImage.sprite = newLocation;

        PutDownPiece();
        onEnterGame.NextTurn();
    }

    private static void Move(GameObject target, Vector2Int from, Vector2Int to)
    {
        // SetLocationData
        GameInfo.Move(from, to);
        target.GetComponent<PieceInfo>().piece.location = to;
        target.transform.parent = boardCanvas.Find(InfoLoader.Vec2ToString(to));
        target.transform.localPosition = Vector3.Lerp(target.transform.localPosition, new Vector3(0, 0, target.transform.position.z), speed);
        GameObject fromObject = boardSetup.pieces[from];
        boardSetup.pieces.Remove(from);
        boardSetup.pieces.Add(to, fromObject);
        pieceInfo.trigger.AfterMove();
    }

    public static void Move(Piece piece, Vector2Int from, Vector2Int to)
    {
        Move(boardSetup.pieces[piece.location], from, to);
    }

    private List<Vector2Int> ValidLoc(Collider obj)
    {
        int x = (int)Mathf.Floor(obj.transform.position.x / scale);
        int y = (int)Mathf.Floor(obj.transform.position.y / scale);
        return ValidLoc(x, y, obj.GetComponent<PieceInfo>().GetPieceType());  //GameInfo.board[new Vector2Int(x, y)].GetPieceType()
    }
    public static List<Vector2Int> ValidLoc(int x, int y, string type, bool link = false)
    {
        switch (type)
        {
            case "General":
                return GeneralLoc(x, y);
            case "Advisor":
                return AdvisorLoc(x, y, link);
            case "Elephant":
                return ElephantLoc(x, y, link);
            case "Horse":
                return HorseLoc(x, y, link);
            case "Chariot":
                return ChariotLoc(x, y, link);
            case "Cannon":
                if (link) return CannonTarget(x, y, true);
                return CannonLoc(x, y);
            case "Soldier":
                return SoldierLoc(x, y, link);
        }
        return new List<Vector2Int>();
    }
    public static List<Vector2Int> GeneralLoc(int x, int y)
    {
        List<Vector2Int> validLoc = new List<Vector2Int>();
        for (int i = -1; i <= 1; i += 2)
        {
            if (boardAttributes.InPalace(x, y + i) && Placeable(x, y + i))
                validLoc.Add(new Vector2Int(x, y + i));
            if (boardAttributes.InPalace(x + i, y) && Placeable(x + i, y))
                validLoc.Add(new Vector2Int(x + i, y));
        }
        return validLoc;
    }
    public static List<Vector2Int> AdvisorLoc(int x, int y, bool link = false)
    {
        List<Vector2Int> validLoc = new List<Vector2Int>();
        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = -1; j <= 1; j += 2)
                if (boardAttributes.InPalace(x + i, y + j) && (Placeable(x + i, y + j) ^ link))
                    validLoc.Add(new Vector2Int(x + i, y + j));
        }
        return validLoc;
    }
    public static List<Vector2Int> ElephantLoc(int x, int y, bool link = false)
    {
        List<Vector2Int> validLoc = new List<Vector2Int>();
        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = -1; j <= 1; j += 2)
                if (boardAttributes.InAllyField(x + i * 2, y + j * 2) && Ignorable(x + i, y + j) && (Placeable(x + i * 2, y + j * 2) ^ link))
                    validLoc.Add(new Vector2Int(x + i * 2, y + j * 2));
        }
        return validLoc;
    }
    public static List<Vector2Int> HorseLoc(int x, int y, bool link = false)
    {
        List<Vector2Int> validLoc = new List<Vector2Int>();
        for (int i = -1; i <= 1; i += 2)
        {
            if (boardAttributes.InBoard(x, y + i) && Ignorable(x, y + i))
                for (int j = -1; j <= 1; j += 2)
                    if (boardAttributes.InBoard(x + j, y + i * 2) && (Placeable(x + j, y + i * 2) ^ link))
                        validLoc.Add(new Vector2Int(x + j, y + i * 2));
            if (boardAttributes.InBoard(x + i, y) && Ignorable(x + i, y))
                for (int j = -1; j <= 1; j += 2)
                    if (boardAttributes.InBoard(x + i * 2, y + j) && (Placeable(x + i * 2, y + j) ^ link))
                        validLoc.Add(new Vector2Int(x + i * 2, y + j));
        }
        return validLoc;
    }
    public static List<Vector2Int> ChariotLoc(int x, int y, bool link = false)
    {
        List<Vector2Int> validLoc = new List<Vector2Int>();
        for (int j = y + 1; j < boardAttributes.boardHeight; j++)
        {
            switch (FindAt(x, j))
            {
                case 'A':
                    if (link) validLoc.Add(new Vector2Int(x, j));
                    break;
                case 'B':
                    if (!link) validLoc.Add(new Vector2Int(x, j));
                    continue;
                case 'E':
                    if (!link) validLoc.Add(new Vector2Int(x, j));
                    break;
                case 'F':
                    continue;
                case 'T':
                    if (!link) validLoc.Add(new Vector2Int(x, j));
                    continue;
            }
            break;
        }
        for (int j = y - 1; j >= 0; j--)
        {
            switch (FindAt(x, j))
            {
                case 'A':
                    if (link) validLoc.Add(new Vector2Int(x, j));
                    break;
                case 'B':
                    if (!link) validLoc.Add(new Vector2Int(x, j));
                    continue;
                case 'E':
                    if (!link) validLoc.Add(new Vector2Int(x, j));
                    break;
                case 'F':
                    continue;
                case 'T':
                    if (!link) validLoc.Add(new Vector2Int(x, j));
                    continue;
            }
            break;
        }
        for (int i = x - 1; i >= 0; i--)
        {
            switch (FindAt(i, y))
            {
                case 'A':
                    if (link) validLoc.Add(new Vector2Int(i, y));
                    break;
                case 'B':
                    if (!link) validLoc.Add(new Vector2Int(i, y));
                    continue;
                case 'E':
                    if (!link) validLoc.Add(new Vector2Int(i, y));
                    break;
                case 'F':
                    continue;
                case 'T':
                    if (!link) validLoc.Add(new Vector2Int(i, y));
                    continue;
            }
            break;
        }
        for (int i = x + 1; i < boardAttributes.boardWidth; i++)
        {
            switch (FindAt(i, y))
            {
                case 'A':
                    if (link) validLoc.Add(new Vector2Int(i, y));
                    break;
                case 'B':
                    if (!link) validLoc.Add(new Vector2Int(i, y));
                    continue;
                case 'E':
                    if (!link) validLoc.Add(new Vector2Int(i, y));
                    break;
                case 'F':
                    continue;
                case 'T':
                    if (!link) validLoc.Add(new Vector2Int(i, y));
                    continue;
            }
            break;
        }
        return validLoc;
    }
    public static List<Vector2Int> CannonLoc(int x, int y)
    {
        List<Vector2Int> validLoc = new List<Vector2Int>();
        for (int j = y + 1; j < boardAttributes.boardHeight; j++)
        {
            if (Ignorable(x, j)) validLoc.Add(new Vector2Int(x, j));
            else
            {
                for (int jj = j + 1; jj < boardAttributes.boardHeight; jj++)
                {
                    switch (FindAt(x, jj))
                    {
                        case 'A':
                            break;
                        case 'B':
                            continue;
                        case 'E':
                            validLoc.Add(new Vector2Int(x, jj));
                            break;
                        case 'F':
                            continue;
                        case 'T':
                            continue;

                    }
                    break;
                }
                break;
            }
        }
        for (int j = y - 1; j >= 0; j--)
        {
            if (Ignorable(x, j)) validLoc.Add(new Vector2Int(x, j));
            else
            {
                for (int jj = j - 1; jj >= 0; jj--)
                {
                    switch (FindAt(x, jj))
                    {
                        case 'A':
                            break;
                        case 'B':
                            continue;
                        case 'E':
                            validLoc.Add(new Vector2Int(x, jj));
                            break;
                        case 'F':
                            continue;
                        case 'T':
                            continue;
                    }
                    break;
                }
                break;
            }
        }
        for (int i = x - 1; i >= 0; i--)
        {
            if (Ignorable(i, y)) validLoc.Add(new Vector2Int(i, y));
            else
            {
                for (int ii = i - 1; ii >= 0; ii--)
                {
                    switch (FindAt(ii, y))
                    {
                        case 'A':
                            break;
                        case 'B':
                            continue;
                        case 'E':
                            validLoc.Add(new Vector2Int(ii, y));
                            break;
                        case 'F':
                            continue;
                        case 'T':
                            continue;
                    }
                    break;
                }
                break;
            }
        }
        for (int i = x + 1; i < boardAttributes.boardWidth; i++)
        {
            if (Ignorable(i, y)) validLoc.Add(new Vector2Int(i, y));
            else
            {
                for (int ii = i + 1; ii < boardAttributes.boardWidth; ii++)
                {
                    switch (FindAt(ii, y))
                    {
                        case 'A':
                            break;
                        case 'B':
                            continue;
                        case 'E':
                            validLoc.Add(new Vector2Int(ii, y));
                            break;
                        case 'F':
                            continue;
                        case 'T':
                            continue;
                    }
                    break;
                }
                break;
            }
        }
        return validLoc;
    }
    public static List<Vector2Int> CannonScope(int x, int y)
    {
        List<Vector2Int> validLoc = new List<Vector2Int>();
        for (int j = y + 1; j < boardAttributes.boardHeight; j++)
        {
            if (!Ignorable(x, j))
            {
                validLoc.Add(new Vector2Int(x, j));
                break;
            }
        }
        for (int j = y - 1; j >= 0; j--)
        {
            if (!Ignorable(x, j)){
                validLoc.Add(new Vector2Int(x, j));
                break;
            }
        }
        for (int i = x - 1; i >= 0; i--)
        {
            if (!Ignorable(i, y)){
                validLoc.Add(new Vector2Int(i, y));
                break;
            }
        }
        for (int i = x + 1; i < boardAttributes.boardWidth; i++)
        {
            if (!Ignorable(i, y))
            {
                validLoc.Add(new Vector2Int(i, y));
                break;
            }
        }
        return validLoc;
    }
    public static List<Vector2Int> CannonTarget(int x, int y, bool link = false)
    {
        List<Vector2Int> validLoc = new List<Vector2Int>();
        for (int j = y + 1; j < boardAttributes.boardHeight; j++)
        {
            if (!Ignorable(x, j))
            {
                for (int jj = j + 1; jj < boardAttributes.boardHeight; jj++)
                {
                    switch (FindAt(x, jj))
                    {
                        case 'A':
                            if (link) validLoc.Add(new Vector2Int(x, jj));
                            break;
                        case 'B':
                            continue;
                        case 'E':
                            if (!link) validLoc.Add(new Vector2Int(x, jj));
                            break;
                        case 'F':
                            continue;
                        case 'T':
                            continue;
                    }
                    break;
                }
                break;
            }
        }
        for (int j = y - 1; j >= 0; j--)
        {
            if (!Ignorable(x, j))
            {
                for (int jj = j - 1; jj >= 0; jj--)
                {
                    switch (FindAt(x, jj))
                    {
                        case 'A':
                            if (link) validLoc.Add(new Vector2Int(x, jj));
                            break;
                        case 'B':
                            continue;
                        case 'E':
                            if (!link) validLoc.Add(new Vector2Int(x, jj));
                            break;
                        case 'F':
                            continue;
                        case 'T':
                            continue;
                    }
                    break;
                }
                break;
            }
        }
        for (int i = x - 1; i >= 0; i--)
        {
            if (!Ignorable(i, y))
            {
                for (int ii = i - 1; ii >= 0; ii--)
                {
                    switch (FindAt(ii, y))
                    {
                        case 'A':
                            if (link) validLoc.Add(new Vector2Int(ii, y));
                            break;
                        case 'B':
                            continue;
                        case 'E':
                            if (!link) validLoc.Add(new Vector2Int(ii, y));
                            break;
                        case 'F':
                            continue;
                        case 'T':
                            continue;
                    }
                    break;
                }
                break;
            }
        }
        for (int i = x + 1; i < boardAttributes.boardWidth; i++)
        {
            if (!Ignorable(i, y))
            {
                for (int ii = i + 1; ii < boardAttributes.boardWidth; ii++)
                {
                    switch (FindAt(ii, y))
                    {
                        case 'A':
                            if (link) validLoc.Add(new Vector2Int(ii, y));
                            break;
                        case 'B':
                            continue;
                        case 'E':
                            if (!link) validLoc.Add(new Vector2Int(ii, y));
                            break;
                        case 'F':
                            continue;
                        case 'T':
                            continue;
                    }
                    break;
                }
                break;
            }
        }
        return validLoc;
    }
    public static List<Vector2Int> SoldierLoc(int x, int y, bool link = false)
    {
        List<Vector2Int> validLoc = new List<Vector2Int>();
        if (boardAttributes.InBoard(x, y + 1) && (FindAt(x, y + 1) != 'A' ^ link))
            validLoc.Add(new Vector2Int(x, y + 1));
        if (!boardAttributes.InAllyField(x, y))
            for (int i = -1; i <= 1; i += 2)
                if (boardAttributes.InBoard(x + i, y) && (FindAt(x + i, y) != 'A' ^ link))
                    validLoc.Add(new Vector2Int(x + i, y));
        return validLoc;
    }
    public static bool IsLink(Piece piece, List<Vector2Int> locations)
    {
        string type = piece.GetPieceType();
        Vector2Int location = piece.location;
        foreach (Piece ally in GameInfo.activeAllies)
            if (ally.GetPieceType() == type && locations.Contains(ally.location) && boardSetup.pieces[ally.location].GetComponent<PieceInfo>().trigger.ValidLoc(true).Contains(location))
                return true;
        return false;
    }

    private static char FindAt(float x, float y) { return FindAt(new Vector2Int((int)x, (int)y)); }
    public static char FindAt(int x, int y) { return FindAt(new Vector2Int(x, y)); }
    private static char FindAt(Vector2Int loc)
    {
        if (GameInfo.board.ContainsKey(loc))
        {
            if (GameInfo.board[loc].isAlly) return 'A'; // Ally
            else return 'E'; // Enemy
        }
        else if (GameInfo.flags.ContainsKey(loc))
        {
            if (GameInfo.flags[loc] == InfoLoader.user.playerID) return 'T';   // True
            else return 'F'; // False
        }
        return 'B';
    }
    private static bool Placeable(int x, int y)
    {
        char result = FindAt(x, y);
        return result != 'A' || result == 'F';
    }
    private static bool Ignorable(int x, int y)
    {
        char result = FindAt(x, y);
        return result == 'B' || result == 'F' || result == 'T';
    }
}