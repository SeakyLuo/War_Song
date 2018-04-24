using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class OnEnterRecruitment : MonoBehaviour, IPointerClickHandler {

    public static UserInfo user;
    public GameObject contractStore, coinStore, popupInputAmountWindow, infoPanel;
    public Text playerCoinsAmount;

    private Camera canvasCamera;
    private RectTransform rectTransform;
    private GameObject[] closeObjects;
    private ContractsManager contractsManager;

	// Use this for initialization
	void Start () {
        canvasCamera = gameObject.GetComponent<Canvas>().worldCamera;
        rectTransform = gameObject.GetComponent<RectTransform>();
        playerCoinsAmount.text = InfoLoader.user.coins.ToString();
        closeObjects = new GameObject[] { infoPanel, contractStore, coinStore };
    }
	
    public void Back()
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
        foreach(GameObject close in closeObjects)
        {
            if (close.activeSelf)
            {
                Vector3 mouseposition = AdjustedMousePosition();
                Rect rect = close.GetComponent<RectTransform>().rect;
                if (-rect.width / 2 > mouseposition.x || mouseposition.x > rect.width / 2 || -rect.height / 2 > mouseposition.y || mouseposition.y > rect.height / 2)
                    close.SetActive(false);
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
