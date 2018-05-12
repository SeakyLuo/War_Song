using UnityEngine;
using UnityEngine.UI;

public class PlayerContract : MonoBehaviour {

    public ContractAttributes attributes;
    public Image image;
    public Text counter;

    private int count;

	// Use this for initialization
	void Start () {
        if(attributes != null)
            SetAttributes(attributes);
    }

    public void SetAttributes(ContractAttributes contractAttributes)
    {
        attributes = contractAttributes;
        gameObject.name = attributes.Name;
        image.sprite = attributes.image;
    }

    public void SetCount(int number)
    {
        gameObject.SetActive(number != 0);
        if (gameObject.activeSelf)
        {
            count = number;
            counter.text = number.ToString();
            counter.transform.parent.gameObject.SetActive(number > 1);
        }
    }

    public int GetCount() { return count; }
}
