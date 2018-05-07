using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour {

    [HideInInspector] public PieceAttributes piece;
    [HideInInspector] public TacticAttributes tactic;

    public Text nameText, descriptionText, costText, healthText, coinText, typeText;
    public Image image, background;
    public GameObject healthImage, coinImage;
    public Sprite allyBackground, enemyBackground;

    private string cardName, type, description;
    private int health = 1;
    private bool isAlly = true;

    public void SetAttributes(CardInfo cardInfo)
    {
        if (cardInfo == null) return;
        if (cardInfo.tactic != null) SetAttributes(cardInfo.tactic);
        else if (cardInfo.piece != null)
        {
            SetAttributes(cardInfo.piece);
            health = cardInfo.GetHealth();
            if (health == 0) healthText.text = "∞";
            else healthText.text = health.ToString();
            healthText.color = cardInfo.healthText.color;
        }
        isAlly = cardInfo.isAlly;
        background.sprite = cardInfo.background.sprite;
    }

    public void SetAttributes(Collection collection)
    {
        if (collection == null) return;
        if (collection.type == "Tactic")
            SetAttributes(Resources.Load<TacticAttributes>("Tactics/" + collection.name + "/Attributes"));
        else
        {
            SetAttributes(Resources.Load<PieceAttributes>("Pieces/" + collection.name + "/Attributes"));
            SetHealth(collection.health);
        }
    }

    // Need to highlight keywords

    public void SetAttributes(PieceAttributes attributes)
    {
        if (attributes == null) return;
        tactic = null;
        piece = attributes;
        nameText.text = attributes.Name;
        cardName = attributes.Name;
        description = attributes.description;
        descriptionText.text = attributes.description;
        costText.text = attributes.oreCost.ToString();
        health = attributes.health;
        healthImage.SetActive(true);
        coinImage.SetActive(false);
        if (health == 0) healthText.text = "∞";
        else healthText.text = attributes.health.ToString();
        healthText.color = Color.white;
        image.sprite = attributes.image;
        type = attributes.type;
        typeText.text = type;
    }

    public void SetAttributes(TacticAttributes attributes)
    {
        if (attributes == null) return;
        piece = null;
        tactic = attributes;
        nameText.text = attributes.Name;
        cardName = attributes.Name;
        description = attributes.description;
        descriptionText.text = attributes.description;
        costText.text = attributes.oreCost.ToString();
        healthImage.SetActive(false);
        coinImage.SetActive(true);
        health = attributes.goldCost;
        coinText.text = attributes.goldCost.ToString();
        image.sprite = attributes.image;
        type = "Tactic";
        typeText.text = type;
    }

    public void SetHealth(int Health)
    {
        if (piece == null) return;
        health = Health;
        if (health == 0) healthText.text = "∞";
        else healthText.text = health.ToString();
        if (piece.health > health) healthText.color = Color.red;
        else if (piece.health == health) healthText.color = Color.white;
        else healthText.color = Color.green;
    }

    public void Clear()
    {
        piece = null;
        tactic = null;
        nameText.text = "Name";
        descriptionText.text = "Description";
        costText.text = "0";
        healthText.text = "0";
        healthText.color = Color.white;
        coinText.text = "0";
        image.sprite = null;
        cardName = type = description = "";
    }

    public string GetCardName() { return cardName; }
    public string GetCardType() { return type; }
    public int GetHealth() { return health; }
    public string GetDescription() { return description; }
    public bool IsStandard() { return cardName.StartsWith("Standard "); }
    public void SetIsAlly(bool value)
    {
        isAlly = value;
        if(value) background.sprite = allyBackground;
        else background.sprite = enemyBackground;
    }
}
