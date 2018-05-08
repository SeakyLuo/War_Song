using UnityEngine;
using UnityEngine.UI;

public class TacticInfo : MonoBehaviour {

    [HideInInspector] public TacticAttributes tactic;
    public Text nameText, oreCostText, goldCostText;
    public Image image;
    [HideInInspector] public int oreCost;
    [HideInInspector] public int goldCost;
    [HideInInspector] public TacticTrigger trigger;
    [HideInInspector] public bool active = true;

    public void SetAttributes(TacticAttributes attributes)
    {
        tactic = attributes;
        nameText.text = attributes.Name;
        SetOreCost(attributes.oreCost);
        goldCostText.text = attributes.goldCost.ToString();
        image.sprite = attributes.image;
        trigger = tactic.trigger;        
    }

    public void SetOreCost(int value)
    {
        oreCost = value;
        if(trigger != null) trigger.oreCost = oreCost;
        oreCostText.text = oreCost.ToString();
    }
    public void ChangeOreCost(int deltaAmount)
    {
        SetOreCost(oreCost + deltaAmount);
    }
    public void SetGoldCost(int value)
    {
        goldCost = value;
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
    }
}
