using UnityEngine;
using UnityEngine.UI;

public class TacticInfo : MonoBehaviour {

    public Text nameText, oreCostText, goldCostText;
    public Image image;
    [HideInInspector] public TacticAttributes tacticAttributes;
    [HideInInspector] public TacticTrigger trigger;
    [HideInInspector] public Tactic tactic;

    public void SetAttributes(TacticAttributes attributes, int ownerID = 0, bool original = true)
    {
        tacticAttributes = attributes;
        if (ownerID == 0) ownerID = Login.playerID;
        tactic = new Tactic(attributes.Name, attributes.oreCost, attributes.goldCost, ownerID, original);
        if (tacticAttributes.trigger != null) trigger = Instantiate(tacticAttributes.trigger);
        if (tacticAttributes.trigger != null) trigger.tactic = tactic;
        nameText.text = attributes.Name;
        SetOreCost(attributes.oreCost);
        SetGoldCost(attributes.goldCost);
        image.sprite = attributes.image;
    }

    public void SetOreCost(int value)
    {
        if (tacticAttributes.trigger != null) trigger.tactic.oreCost = value;
        tactic.oreCost = value;
        oreCostText.text = value.ToString();
    }
    public void ChangeOreCost(int deltaAmount)
    {
        SetOreCost(tactic.oreCost + deltaAmount);
    }
    public void SetGoldCost(int value)
    {
        if (tacticAttributes.trigger != null) trigger.tactic.goldCost = value;
        tactic.goldCost = value;
        goldCostText.text = value.ToString();
    }
    public void ChangeGoldCost(int deltaAmount)
    {
        SetOreCost(tactic.goldCost + deltaAmount);
    }

    public void Clear()
    {
        tactic = null;
        trigger = null;
        nameText.text = "Tactic";
        oreCostText.text = "0";
        goldCostText.text = "0";
        image.sprite = null;
    }
}
