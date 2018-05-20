using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOverLineup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject delete;

    public void OnPointerEnter(PointerEventData eventData)
    {
        delete.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        delete.SetActive(false);
    }

}
