using UnityEngine;
using UnityEngine.UI;

public class TacticInfo : MonoBehaviour {

    [HideInInspector] public TacticAttributes tactic;
    public Text Name, oreCost, goldCost;
    public Image image;
    [HideInInspector] public Trigger trigger;
    [HideInInspector] public bool active = true;

    public void SetAttributes(TacticAttributes attributes)
    {
        tactic = attributes;
        Name.text = attributes.Name;
        oreCost.text = attributes.oreCost.ToString();
        goldCost.text = attributes.goldCost.ToString();
        image.sprite = attributes.image;
        trigger = tactic.trigger;
    }

    public void Clear()
    {
        tactic = null;
        Name.text = "Tactic";
        oreCost.text = "0";
        goldCost.text = "0";
        image.sprite = null;
    }
}
