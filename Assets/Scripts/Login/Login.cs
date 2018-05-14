using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Login : MonoBehaviour
{
    public InputField inputEmail, inputPassword;
    public Text connectingDots;
    public GameObject createAccountPanel, emptyEmail, wrongPassword, emptyPassword;
    public GameObject settingsPanel, forgotPasswordPanel, networkError, connecting;

    // better to support phone number registration

    // Use this for initialization
    void Start () {
        // If already has an account saved
        string email = PlayerPrefs.GetString("email"),
                password = PlayerPrefs.GetString("password");
        if (email != "" && password != "")
            login(email, password);
            //StartCoroutine(RequestLogin(email, password, false));
	}

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            settingsPanel.SetActive(true);
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
        //StartCoroutine(RequestLogin(inputEmail.text, inputPassword.text));
    }

    public IEnumerator RequestLogin(string email, string password, bool showError = true)  //connect with server, and VERIFY credentials
    {
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("email", email);
        infoToPhp.AddField("password", password);
        gameObject.SetActive(false);
        StartCoroutine(ChangeConnectingDots());

        WWW sendToPhp = new WWW("http://localhost:8888/action_login.php", infoToPhp);
        yield return sendToPhp;
        StopAllCoroutines();

        if (string.IsNullOrEmpty(sendToPhp.error)) //if no error connecting to server
        {
            if (sendToPhp.text.Contains("invalid creds"))  //if credentials don't exist 
            {
                inputPassword.text = "";
                wrongPassword.SetActive(true);
            }
            else                                           //connection and credentials success
            {
                PlayerPrefs.SetString("email", email);
                PlayerPrefs.SetString("password", password);
                if (emptyEmail.activeSelf) emptyEmail.SetActive(false);
                if (emptyPassword.activeSelf) emptyPassword.SetActive(false);
                if (wrongPassword.activeSelf) wrongPassword.SetActive(false);
                SceneManager.LoadScene("Main");
            }
        }
        else                                               //connection failure
        {
            if(showError) networkError.SetActive(true);
        }
        gameObject.SetActive(true);
    }

    private void login(string email, string password)
    {
        if(email == "1@1.com" && password == "12345678")
        {
            PlayerPrefs.SetString("email", email);
            PlayerPrefs.SetString("password", password);
            if (emptyEmail.activeSelf) emptyEmail.SetActive(false);
            if (emptyPassword.activeSelf) emptyPassword.SetActive(false);
            if (wrongPassword.activeSelf) wrongPassword.SetActive(false);
            SceneManager.LoadScene("Main");
        }
    }

    private IEnumerator ChangeConnectingDots()
    {
        int count = 0;
        while (true)
        {
            connectingDots.text = "Connecting" + new string('*', count % 6 + 1);
            yield return new WaitForSeconds(0.5f);
        }
    }
    
}
