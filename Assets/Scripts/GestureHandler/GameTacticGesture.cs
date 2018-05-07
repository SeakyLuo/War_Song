using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameTacticGesture : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    public GameObject infoCard;

    //private Vector3 newPosition;
    private GameObject tactic;
    private Trigger trigger;
    private float prevClick = 0;
    private float doubleClickInterval = 1;

    private void Start()
    {
        //newPosition = new Vector3(300, transform.localPosition.y, -6.1f);  // I don't know why the fuck is this -360
        tactic = transform.Find("Tactic").gameObject;
        trigger = tactic.GetComponent<TacticInfo>().trigger;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Input.GetMouseButtonUp(1)) ActivateAbility.RemoveTargets();
        else if (Input.GetMouseButtonUp(0))
        {
            if (Time.time - prevClick < doubleClickInterval)
            {
                if (trigger != null) trigger.Activate();
            }
            else
            {
                List<Vector2Int> targets = trigger.ValidTarget();
                if (targets.Count != 0) ActivateAbility.ShowTacticTarget(targets);
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
        infoCard.transform.localPosition = new Vector3(300, transform.localPosition.y, -6.1f); // So I have to do this
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoCard.activeSelf)
            infoCard.SetActive(false);
    }

}
