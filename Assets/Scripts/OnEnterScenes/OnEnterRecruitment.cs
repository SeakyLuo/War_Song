using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class OnEnterRecruitment : MonoBehaviour, IPointerClickHandler {

    public static UserInfo user;
    public GameObject contractStore, coinStore, popupInputAmountWindow, infoPanel, settingsPanel;
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

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            settingsPanel.SetActive(true);
    }

    public void Back()
    {
        SceneManager.LoadScene("Main");
    }

    public void OpenContractStoreWindow()
    {
        contractStore.SetActive(true);
        coinStore.SetActive(false);
    }

    public void OpenCoinStoreWindow()
    {
        contractStore.SetActive(false);
        coinStore.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach(GameObject close in closeObjects)
        {
            if (close.activeSelf)
            {
                Vector3 mousePosition = AdjustedMousePosition();
                Rect rect = close.GetComponent<RectTransform>().rect;
                if (mousePosition.x < rect.x || mousePosition.x > -rect.x || mousePosition.y < rect.y || mousePosition.y > -rect.y)
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
