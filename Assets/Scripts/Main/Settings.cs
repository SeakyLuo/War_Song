using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour, IPointerClickHandler
{
    public GameObject mainSettingsPanel, optionsPanel, logoutPanel, creditsPanel, guidebookPanel;
    public Canvas canvas;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject close = mainSettingsPanel;
        if (optionsPanel.activeSelf) close = optionsPanel;
        else if (GuideBookPanelActive()) close = guidebookPanel;
        Vector2 mousePosition = AdjustedMousePosition();
        Rect rect = close.GetComponent<RectTransform>().rect;
        // rect.x and rect.y are negative
        if (mousePosition.x < rect.x || mousePosition.x > -rect.x || mousePosition.y < rect.y || mousePosition.y > -rect.y)
        {
            if (optionsPanel.activeSelf || GuideBookPanelActive()) close.SetActive(false);
            else gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (CreditPanelActive()) creditsPanel.SetActive(false);
            else if (optionsPanel.activeSelf) optionsPanel.SetActive(false);
            else if (GuideBookPanelActive()) guidebookPanel.SetActive(false);
            else if (LogoutPanelActive()) return;
            else
            {
                gameObject.SetActive(!gameObject.activeSelf);
                //if(SceneManager.GetActiveScene().name == "GameMode" && gameObject.activeSelf) MovementController.PutDownPiece();
            }
        }
    }

    public void ShowSettings()
    {
        if (CreditPanelActive())
        {
            creditsPanel.SetActive(false);
            optionsPanel.SetActive(false);            
        }
        gameObject.SetActive(!gameObject.activeSelf);
        if(SceneManager.GetActiveScene().name == "GameMode") MovementController.PutDownPiece();
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

    private bool LogoutPanelActive() { return logoutPanel != null && logoutPanel.activeSelf; }
    private bool CreditPanelActive() { return creditsPanel != null && creditsPanel.activeSelf; }
    private bool GuideBookPanelActive() { return guidebookPanel != null && guidebookPanel.activeSelf; }

    private Vector2 AdjustedMousePosition()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out mousePosition);
        return mousePosition;
    }
}
