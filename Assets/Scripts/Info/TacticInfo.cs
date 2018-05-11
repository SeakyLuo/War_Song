using UnityEngine;
using UnityEngine.UI;

public class TacticInfo : MonoBehaviour {

    public Text nameText, oreCostText, goldCostText;
    public Image image;
    [HideInInspector] public TacticAttributes tactic;
    [HideInInspector] public TacticTrigger trigger;
    [HideInInspector] public int oreCost;
    [HideInInspector] public int goldCost;
    [HideInInspector] public bool active = true;

    public void SetAttributes(TacticAttributes attributes)
    {
        tactic = attributes;
        trigger = tactic.trigger;
        nameText.text = attributes.Name;
        SetOreCost(attributes.oreCost);
        SetGoldCost(attributes.goldCost);
        image.sprite = attributes.image;
    }

    public void SetOreCost(int value)
    {
        oreCost = value;
        trigger.oreCost = oreCost;
        oreCostText.text = oreCost.ToString();
    }
    public void ChangeOreCost(int deltaAmount)
    {
        SetOreCost(oreCost + deltaAmount);
    }
    public void SetGoldCost(int value)
    {
        goldCost = value;
        trigger.goldCost = goldCost;
        goldCostText.text = goldCost.ToString();
    }
    public void ChangeGoldCost(int deltaAmount)
    {
        SetOreCost(goldCost + deltaAmount);
    }

    public void Clear()
    {
        tactic = null;
        nameText.text = "Tactic";
        oreCostText.text = "0";
        goldCostText.text = "0";
        image.sprite = null;
        oreCost = goldCost = 0;
    }
}
