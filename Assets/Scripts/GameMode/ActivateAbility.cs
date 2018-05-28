using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateAbility : MonoBehaviour {

    public static OnEnterGame onEnterGame;
    public static bool activated = false;
    public static int tacticCaller = -1;
    public static List<Location> targetLocs = new List<Location>();
    public static PieceInfo pieceInfo;
    public static Button button;
    public static string actor;

    public GameController gameController;
    public GameObject invalidTarget;
    public GameObject tacticBag;

    private static Text text;
    private static GameObject textObj;
    private static GameObject targetDot;
    private static Transform board;
    private static TacticTrigger tacticTrigger;
    private static List<GameObject> targetDots = new List<GameObject>();

	// Use this for initialization
	void Start () {
        button = GetComponent<Button>();
        textObj = transform.Find("Text").gameObject;
        text = textObj.GetComponent<Text>();
        onEnterGame = transform.parent.parent.GetComponent<OnEnterGame>();
        targetDot = onEnterGame.targetDot;
        board = onEnterGame.board.transform.parent;
    }

    public static void Activate(Location location)
    {
        if (tacticCaller != -1)
        {
            // use tactic
            if (!GameController.ChangeOre(-tacticTrigger.tactic.oreCost) || !GameController.ChangeCoin(-tacticTrigger.tactic.goldCost)) return;
            tacticTrigger.Activate(location);
            GameController.RemoveTactic(tacticTrigger.tactic, true);
            tacticCaller = -1;
        }
        else
        {
            // activate ability
            if (!GameController.ChangeOre(-pieceInfo.trigger.piece.oreCost)) return;
            pieceInfo.trigger.Activate(location);
            MovementController.PutDownPiece();
        }
        DeactivateButton();
    }

    private IEnumerator ShowInvalidTarget()
    {
        invalidTarget.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        invalidTarget.SetActive(false);
    }

    public void ButtonDrawTargets()
    {
        OnEnterGame.CancelTacticHighlight();
        if (targetLocs.Count == 0)
        {
            // if not targets, trigger directly
            if (!GameController.ChangeOre(-pieceInfo.trigger.piece.oreCost)) return;
            pieceInfo.trigger.Activate();
            MovementController.PutDownPiece();
            OnEnterGame.gameInfo.Act("ability", Login.playerID);
            if (!OnEnterGame.gameInfo.Actable(Login.playerID)) onEnterGame.NextTurn();
        }
        else DrawTargets();
    }

    public static void DrawTargets()
    {
        activated = true;
        MovementController.HidePathDots();
        foreach (Location loc in targetLocs)
        {
            GameObject copy = Instantiate(targetDot, board);
            copy.name = loc.ToString();
            copy.transform.position = new Vector3(loc.x * MovementController.scale, loc.y * MovementController.scale, -2.5f);
            targetDots.Add(copy);
        }
    }

    public static void RemoveTargets()
    {
        foreach (GameObject targetDot in targetDots) Destroy(targetDot);
        targetDots.Clear();
    }

    public static void ShowTacticTarget(List<Location> validTargets, int caller, TacticTrigger trigger)
    {
        actor = "tactic";
        targetLocs = validTargets;
        tacticCaller = caller;
        tacticTrigger = trigger;
        DrawTargets();
    }

    public static void ActivateButton()
    {
        OnEnterGame.CancelTacticHighlight();
        if (pieceInfo.trigger != null && !pieceInfo.trigger.Activatable()) return;
        actor = "ability";
        targetLocs = pieceInfo.ValidTarget();
        button.interactable = true;
        textObj.SetActive(true);
        if (targetLocs.Count == 0) text.text = "Activate\nAbility";
        else text.text = "Show\nTargets";
    }

    public static void DeactivateButton()
    {
        actor = "";
        pieceInfo = null;
        activated = false;
        button.interactable = false;
        textObj.SetActive(false);
        if (targetLocs.Count == 0) return;
        RemoveTargets();
    }
}
