using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class OnEnterRecruitment : MonoBehaviour, IPointerClickHandler {

    public static UserInfo user;
    public GameObject contractStore, coinStore, popupInputAmountWindow;
    public Text playerCoinsAmount;

    private Camera canvasCamera;
    private RectTransform rectTransform;
    private List<GameObject> stores;
    private ContractsManager contractsManager;

	// Use this for initialization
	void Start () {
        canvasCamera = gameObject.GetComponent<Canvas>().worldCamera;
        rectTransform = gameObject.GetComponent<RectTransform>();
        playerCoinsAmount.text = InfoLoader.user.coins.ToString();
        stores = new List<GameObject>() { contractStore, coinStore };
    }
	
    public void BackToMain()
    {
        SceneManager.LoadScene("Main");
    }

    public void OpenContractStoreWindow()
    {
        contractStore.SetActive(true);
    }

    public void OpenCoinStoreWindow()
    {
        coinStore.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach(GameObject store in stores)
        {
            if (store.activeSelf)
            {
                Vector3 mouseposition = AdjustedMousePosition();
                Rect rect = store.GetComponent<RectTransform>().rect;
                if (-rect.width / 2 > mouseposition.x || mouseposition.x > rect.width / 2 || -rect.height / 2 > mouseposition.y || mouseposition.y > rect.height / 2)
                {
                    store.SetActive(false);
                }
                break;
            }
        }
    }

    private Vector3 AdjustedMousePosition()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, canvasCamera, out mousePosition);
        return mousePosition;
    }
}
