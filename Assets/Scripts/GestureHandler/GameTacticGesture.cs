using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameTacticGesture : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject infoCard;

    private Vector3 newPosition;
    private GameObject tactic;
    private Button button;
    private TacticTrigger trigger;
    private float prevClick = 0;
    private float doubleClickInterval = 1;

    private void Awake()
    {
        newPosition = new Vector3(300, transform.localPosition.y, -6.1f);
    }

    private void Start()
    {
        tactic = transform.Find("Tactic").gameObject;
        button = GetComponent<Button>();
        trigger = tactic.GetComponent<TacticInfo>().trigger;
    }

    public void UseTactic(int caller)
    {
        if (Input.GetMouseButtonUp(1))
        {
            button.GetComponent<Image>().sprite = button.spriteState.disabledSprite;
            ActivateAbility.RemoveTargets();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - prevClick < doubleClickInterval)
            {
                if (trigger != null)
                {
                    trigger.Activate();
                    gameObject.SetActive(false);
                }
            }
            else
            {
                button.GetComponent<Image>().sprite = button.spriteState.highlightedSprite;
                List<Vector2Int> targets = trigger.ValidTarget();
                if (targets.Count != 0) ActivateAbility.ShowTacticTarget(targets, caller, trigger);
            }
            infoCard.SetActive(false);
            prevClick = Time.time;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!tactic.activeSelf) return;
        infoCard.SetActive(true);
        infoCard.GetComponent<CardInfo>().SetAttributes(tactic.GetComponent<TacticInfo>().tactic);
        infoCard.transform.localPosition = newPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoCard.activeSelf)
            infoCard.SetActive(false);
    }

}
