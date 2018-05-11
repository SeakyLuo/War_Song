using UnityEngine;
using UnityEngine.UI;

public class DropdownIcon : MonoBehaviour
{
    public string searchType;
    public Text label;
    public CollectionManager collectionManager;

    private int value;

    public void SetText(int number)
    {
        value = number;
        if (value != -1) label.text = value.ToString();
        else label.text = "";
        gameObject.SetActive(false);
        if (searchType == "ore") collectionManager.SetSearchByOre(value);
        else if (searchType == "health") collectionManager.SetSearchByHealth(value);
        else if (searchType == "coin") collectionManager.SetSearchByGold(value);
    }

    public void ShowDropdown()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void HideDropdown()
    {
        gameObject.SetActive(false);
    }
}
