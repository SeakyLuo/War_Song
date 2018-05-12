using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameTacticGesture : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject infoCard;

    private GameObject tacticObj;
    private Button button;
    private TacticTrigger trigger;
    private float prevClick = 0;
    private float doubleClickInterval = 1;

    private static List<Vector2Int> targets = new List<Vector2Int>();

    private void Start()
    {
        tacticObj = transform.Find("Tactic").gameObject;
        button = GetComponent<Button>();
        trigger = tacticObj.GetComponent<TacticInfo>().trigger;
    }

    public void UseTactic(int caller)
    {
        if (MovementController.selected != null) MovementController.PutDownPiece();
        if (ActivateAbility.activated) ActivateAbility.DeactivateButton();
        if (OnEnterGame.current_tactic != -1 && targets.Count != 0) Resume();
        else
        {
            // double click doesn't work well
            if (!trigger.needsTarget && Time.time - prevClick < doubleClickInterval)
            {
                if (!GameController.ChangeOre(-trigger.tactic.oreCost) || !GameController.ChangeCoin(-trigger.tactic.goldCost)) return;
                trigger.Activate();
                gameObject.SetActive(false);
            }
            else
            {
                OnEnterGame.current_tactic = caller;
                button.GetComponent<Image>().sprite = button.spriteState.highlightedSprite;
                targets = trigger.ValidTargets();
                if (targets.Count != 0) ActivateAbility.ShowTacticTarget(targets, caller, trigger);
            }
        }
        infoCard.SetActive(false);
        prevClick = Time.time;
    }

    public static void Resume()
    {
        OnEnterGame.CancelTacticHighlight();
        ActivateAbility.RemoveTargets();
        targets.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!tacticObj.activeSelf) return;
        infoCard.SetActive(true);
        CardInfo cardInfo = infoCard.GetComponent<CardInfo>();
        cardInfo.SetAttributes(tacticObj.GetComponent<TacticInfo>().tacticAttributes);
        cardInfo.SetIsAlly(true);
        cardInfo.SetTactic(tacticObj.GetComponent<TacticInfo>().tactic);
        infoCard.transform.localPosition = new Vector3(300, transform.localPosition.y, -6.1f); ;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoCard.activeSelf)
            infoCard.SetActive(false);
    }

}