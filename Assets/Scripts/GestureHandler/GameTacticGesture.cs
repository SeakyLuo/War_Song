using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameTacticGesture : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject infoCard;

    private GameObject tacticObj;
    private Button button;
    private float prevClick = 0;
    private float doubleClickInterval = 1;

    private static List<Location> targets = new List<Location>();

    private void Start()
    {
        tacticObj = transform.Find("Tactic").gameObject;
        button = GetComponent<Button>();
    }

    public void UseTactic(int caller)
    {
        if (MovementController.selected != null) MovementController.PutDownPiece();
        if (ActivateAbility.activated) ActivateAbility.DeactivateButton();
        if (OnEnterGame.current_tactic != -1 && targets.Count != 0) Resume();
        else
        {
            TacticTrigger trigger = tacticObj.GetComponent<TacticInfo>().trigger;
            if (!trigger.needsTarget && Time.time - prevClick < doubleClickInterval)
            {
                if (!GameController.ChangeOre(-trigger.tactic.oreCost) || !GameController.ChangeCoin(-trigger.tactic.goldCost)) return;
                trigger.Activate();                
                GameController.RemoveTactic(trigger.tactic, true);
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
        cardInfo.SetOwner(Login.playerID);
        cardInfo.SetTactic(tacticObj.GetComponent<TacticInfo>().tactic);
        infoCard.transform.localPosition = new Vector3(300, transform.localPosition.y, -6.1f); ;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoCard.activeSelf)
            infoCard.SetActive(false);
    }

}