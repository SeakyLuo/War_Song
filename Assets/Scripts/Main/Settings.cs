using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour
{
    public Canvas parentCanvas;
    public GameObject optionsPanel, logoutPanel;

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void ShowLogout()
    {
        logoutPanel.SetActive(true);
    }

    public void ConfirmLogout()
    {
        SceneManager.LoadScene("Login");
    }

    public void CancelLogout()
    {
        logoutPanel.SetActive(false);
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
