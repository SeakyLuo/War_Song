using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Login : MonoBehaviour, IPointerClickHandler
{
    public InputField inputEmail, inputPassword;
    public GameObject createAccountPanel, emptyEmail, wrongPassword, emptyPassword;
    public GameObject settingsPanel, optionsPanel, forgotPasswordPanel;
    public Canvas parentCanvas;

    private GameObject[] closeObjects;

    //support phone number

    // Use this for initialization
    void Start () {
        closeObjects = new GameObject[] { optionsPanel, settingsPanel };
        // If already has an account saved
        string email = PlayerPrefs.GetString("email"),
                password = PlayerPrefs.GetString("password");
        if (email != "" && password != "")
            login(email, password);
	}

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            ShowSettings();
    }

    public void ConfirmLogin()
    {
        bool emailIsEmpty = (inputEmail.text == ""), passwordIsEmpty = (inputPassword.text == "");
        if(emailIsEmpty || passwordIsEmpty)
        {
            emptyEmail.SetActive(emailIsEmpty);
            emptyPassword.SetActive(passwordIsEmpty);
            return;
        }
        emptyPassword.SetActive(false);
        login(inputEmail.text, inputPassword.text);
    }

    public void login(string email, string password)
    {
        // Connect to the server
        // if not work return and warn
        // else save email and password
        // download data
        // Info Loader
        if (email == "1@1.com" && password == "12345678") // match
        {
            PlayerPrefs.SetString("email", email);
            PlayerPrefs.SetString("password", password);
            if (emptyEmail.activeSelf) emptyEmail.SetActive(false);
            if (emptyPassword.activeSelf) emptyPassword.SetActive(false);
            if (wrongPassword.activeSelf) wrongPassword.SetActive(false);
            SceneManager.LoadScene("Main");
        }
        else
        {
            inputPassword.text = "";
            wrongPassword.SetActive(true);
        }        
    }

    public void ForgotPassword()
    {
        //Send Verification code
        forgotPasswordPanel.SetActive(true);
    }

    public void CreateNewAccount()
    {
        createAccountPanel.SetActive(true);
    }

    public void ShowSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void ShowOptions()
    {
        optionsPanel.SetActive(!optionsPanel.activeSelf);
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
                    close.SetActive(false);
                break;
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
