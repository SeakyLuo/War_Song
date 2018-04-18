using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class SwitchScenes : MonoBehaviour, IPointerClickHandler
{
    public GameObject playerInfoPanel, settingsPanel;
    public Text time;

    private Canvas parentCanvas;
    private GameObject[] closeObjects;

    private void Start()
    {
        parentCanvas = gameObject.GetComponent<Canvas>();
        closeObjects = new GameObject[] { playerInfoPanel, settingsPanel };
    }

    private void FixedUpdate()
    {
        time.text = DateTime.Now.ToString("h:mm tt");
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
    }

    public void ShowSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ChangeQuest()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        foreach(GameObject close in closeObjects)
        {
            if (close.activeSelf)
            {
                Vector2 mousePosition = AdjustedMousePosition();
                Rect rect = close.GetComponent<RectTransform>().rect;
                // rect.x and rect.y are negative
                if (mousePosition.x < rect.x || mousePosition.x > -rect.x || mousePosition.y < rect.y || mousePosition.y > -rect.y)
                    close.SetActive(false);
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
