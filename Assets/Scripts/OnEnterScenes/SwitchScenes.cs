using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SwitchScenes : MonoBehaviour, IPointerClickHandler
{
    public GameObject playerInfoPanel, settingsPanel, optionsPanel;
    private GameObject[] closeObjects;
    public Text winText, loseText, drawText, percentageText;
    public Text rank, title;

    private Canvas parentCanvas;

    private void Start()
    {
        parentCanvas = gameObject.GetComponent<Canvas>();
        closeObjects = new GameObject[] { optionsPanel, settingsPanel, playerInfoPanel };
        winText.text = InfoLoader.user.total.win.ToString();
        loseText.text = InfoLoader.user.total.lose.ToString();
        drawText.text = InfoLoader.user.total.draw.ToString();
        percentageText.text = InfoLoader.user.total.percentage.ToString();
        rank.text = InfoLoader.user.rank.ToString();
        title.text = Range.FindTitle(InfoLoader.user.rank);
    }

    public void EnterCollection()
    {
        SceneManager.LoadScene("Collections");
    }

    public void EnterWar()
    {
        SceneManager.LoadScene("PlayerMatching");
    }

    public void EnterRecruitment()
    {
        SceneManager.LoadScene("Recruitment");
    }

    public void ShowPlayerInfo()
    {
        playerInfoPanel.SetActive(true);
        settingsPanel.SetActive(false);
        settingsPanel.GetComponent<Settings>().optionsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        playerInfoPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ChangeChallenge()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach (GameObject close in closeObjects)
        {
            if (close.activeSelf)
            {
                Vector2 mousePosition = AdjustedMousePosition();
                Rect rect = close.GetComponent<RectTransform>().rect;
                // rect.x and rect.y are negative
                if (mousePosition.x < rect.x || mousePosition.x > -rect.x || mousePosition.y < rect.y || mousePosition.y > -rect.y)
                {
                    close.SetActive(false);
                    break;
                }
            }
        }
    }

    private Vector2 AdjustedMousePosition()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Input.mousePosition, parentCanvas.worldCamera, out mousePosition);
        return mousePosition;
    }
}
