using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MouseOverTactic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject card;

    private Vector3 newPosition;
    private GameObject tactic;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "GameMode")
            newPosition = new Vector3(300, transform.localPosition.y, -6.1f);  // I don't know why the fuck is this -360
        else
            newPosition = new Vector3(transform.position.x + GetComponent<RectTransform>().rect.x + card.GetComponent<RectTransform>().rect.x,
                          transform.position.y - 15);
        tactic = transform.Find("Tactic").gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!tactic.activeSelf) return;
        card.SetActive(true);
        card.GetComponent<CardInfo>().SetAttributes(tactic.GetComponent<TacticInfo>().tactic);
        if (SceneManager.GetActiveScene().name == "GameMode")
            card.transform.localPosition = new Vector3(300, transform.localPosition.y, -6.1f); // So I have to do this
        else
            card.transform.position = newPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(card.activeSelf) card.SetActive(false);
    }

}
