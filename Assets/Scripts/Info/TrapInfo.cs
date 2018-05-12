using UnityEngine;
using UnityEngine.UI;

public class TrapInfo : MonoBehaviour {

    public Text nameText;
    public Text descriptionText;
    public Image background;
    public Image image;
    public Sprite allyBackground;
    public Sprite enemyBackground;

	public void SetAttributes(TrapAttributes trap, int creator)
    {
        nameText.text = trap.name;
        descriptionText.text = trap.description;
        image.sprite = trap.image;
        if (creator == InfoLoader.user.playerID) background.sprite = allyBackground;
        else background.sprite = enemyBackground;
    }
}
