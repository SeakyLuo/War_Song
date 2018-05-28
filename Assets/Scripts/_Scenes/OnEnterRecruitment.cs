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

	// Use this for initialization
	void Start () {
        canvasCamera = GetComponent<Canvas>().worldCamera;
        rectTransform = GetComponent<RectTransform>();
        closeObjects = new GameObject[] { infoPanel, contractStore, coinStore };
        playerCoinsAmount.text = Login.user.coins.ToString();
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
