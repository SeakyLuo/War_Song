using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour, IPointerClickHandler
{
    public GameObject mainSettingsPanel, optionsPanel, logoutPanel, creditsPanel;
    public Canvas parentCanvas;

    public void OnPointerClick(PointerEventData eventData)
    {
        if ( (logoutPanel!=null && logoutPanel.activeSelf) || creditsPanel.activeSelf) return;
        GameObject close = mainSettingsPanel;
        if (optionsPanel.activeSelf) close = optionsPanel;
        Vector2 mousePosition = AdjustedMousePosition();
        Rect rect = close.GetComponent<RectTransform>().rect;
        // rect.x and rect.y are negative
        if (mousePosition.x < rect.x || mousePosition.x > -rect.x || mousePosition.y < rect.y || mousePosition.y > -rect.y)
        {
            if (optionsPanel.activeSelf) close.SetActive(false);
            else gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (creditsPanel.activeSelf) creditsPanel.SetActive(false);
            else if (optionsPanel.activeSelf) optionsPanel.SetActive(false);
            else if (logoutPanel.activeSelf) return;
            else gameObject.SetActive(!gameObject.activeSelf);
        }
    }

    public void ShowSettings()
    {
        if (creditsPanel.activeSelf)
        {
            creditsPanel.SetActive(false);
            optionsPanel.SetActive(false);            
        }
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void ConfirmLogout()
    {
        PlayerPrefs.SetString("email","");
        PlayerPrefs.SetString("password","");
        SceneManager.LoadScene("Login");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private Vector2 AdjustedMousePosition()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Input.mousePosition, parentCanvas.worldCamera, out mousePosition);
        return mousePosition;
    }
}
