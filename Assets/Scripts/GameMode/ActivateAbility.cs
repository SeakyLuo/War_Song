using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateAbility : MonoBehaviour {

    public static bool activated = false;
    public static int tacticCaller = -1;

    public GameController gameController;
    public OnEnterGame onEnterGame;
    public GameObject invalidTarget;
    public GameObject history;
    public GameObject tacticBag;

    private static Button button;
    private static GameObject text;
    private static GameObject targetDot;
    private static Transform board;
    private static PieceInfo pieceInfo;
    private static TacticTrigger tacticTrigger;
    
    private static List<Vector2Int> targetLocs = new List<Vector2Int>();
    private static List<GameObject> targetDots = new List<GameObject>();

	// Use this for initialization
	void Start () {
        button = GetComponent<Button>();
        text = transform.Find("Text").gameObject;
        targetDot = onEnterGame.targetDot;
        board = onEnterGame.board.transform.parent;
    }

    private void Update()
    {
        if (OnEnterGame.gameover || GameInfo.actionTaken || !activated) return;
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Collider hitObj = hit.collider;
                if (hitObj == MovementController.selected) MovementController.PutDownPiece();
                else if (hitObj.name != "UIPanel")
                {
                    Vector2Int location;
                    if (hitObj.name == "Piece") location = InfoLoader.StringToVec2(hitObj.transform.parent.name);
                    else location = InfoLoader.StringToVec2(hitObj.name);
                    if (targetLocs.Contains(location))
                    {
                        if (tacticCaller != -1)
                        {
                            tacticTrigger.Activate();
                            tacticBag.transform.Find("TacticSlot" + tacticCaller).gameObject.SetActive(false);
                            tacticCaller = -1;
                        }
                        else
                        {
                            pieceInfo.trigger.Activate(location);
                            MovementController.PutDownPiece();
                        }
                        AddToHistory();
                        RemoveTargets();
                        onEnterGame.NextTurn();
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(1)) MovementController.PutDownPiece();
    }

    private IEnumerator ShowInvalidTarget()
    {
        invalidTarget.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        invalidTarget.SetActive(false);
    }

    public void ButtonDrawTargets()
    {
        if (targetLocs.Count == 0)
        {
            pieceInfo.trigger.Activate();
            AddToHistory();
            MovementController.PutDownPiece();
            onEnterGame.NextTurn();
        }
        else DrawTargets();
    }

    public static void DrawTargets()
    {
        activated = true;
        MovementController.HidePathDots();
        foreach (Vector2Int loc in targetLocs)
        {
            GameObject copy = Instantiate(targetDot, board);
            copy.name = InfoLoader.Vec2ToString(loc);
            copy.transform.position = new Vector3(loc.x * MovementController.scale, loc.y * MovementController.scale, -1 + MovementController.selected.transform.position.z);
            targetDots.Add(copy);
        }
    }

    public static void RemoveTargets()
    {
        foreach (GameObject targetDot in targetDots) Destroy(targetDot);
        targetDots.Clear();
    }

    public static void ShowTacticTarget(List<Vector2Int> validTargets, int caller, TacticTrigger trigger)
    {
        targetLocs = validTargets;
        tacticCaller = caller;
        tacticTrigger = trigger;
        DrawTargets();
    }

    public static void ActivateButton()
    {
        pieceInfo = MovementController.selected.GetComponent<PieceInfo>();
        if (pieceInfo.trigger != null && !pieceInfo.trigger.Activatable()) return;
        targetLocs = pieceInfo.ValidTarget();
        button.interactable = true;
        text.SetActive(true);
    }

    public static void DeactivateButton()
    {
        pieceInfo = null;
        activated = false;
        button.interactable = false;
        text.SetActive(false);
        if (targetLocs.Count == 0) return;
        RemoveTargets();
    }

    private void AddToHistory()
    {

    }
}
