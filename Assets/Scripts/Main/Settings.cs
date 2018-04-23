using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Settings : MonoBehaviour
{
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
        PlayerPrefs.SetString("email","");
        PlayerPrefs.SetString("password","");
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
}
