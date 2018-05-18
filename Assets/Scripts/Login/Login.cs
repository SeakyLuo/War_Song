using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public static UserInfo user;
    public static int playerID;

    public InputField inputEmail, inputPassword;
    public Text connectingDots;
    public GameObject createAccountPanel, emptyEmail, wrongPassword, emptyPassword;
    public GameObject settingsPanel, forgotPasswordPanel, networkError, connecting;

    private static bool called = false;

    // better to support phone number registration

    void Start () {
        if (!called)
        {
            new Database();
            called = true;
        }
        // If already has an account saved
        string email = PlayerPrefs.GetString("email"),
               password = PlayerPrefs.GetString("password");
        if (email != "" && password != "")
            RequestLogin(email, password, false);
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
        RequestLogin(inputEmail.text, inputPassword.text);
    }

    public void RequestLogin(string email, string password, bool showError = true)  //connect with server, and VERIFY credentials
    {
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("email", email);
        infoToPhp.AddField("password", password);

        connecting.SetActive(true);
        StartCoroutine(ChangeConnectingDots());

        WWW sendToPhp = new WWW("http://47.151.234.225/action_login.php", infoToPhp);
        while (!sendToPhp.isDone) { }
        StopAllCoroutines();

        if (string.IsNullOrEmpty(sendToPhp.error)) //if no error connecting to server
        {
            if (sendToPhp.text.Contains("invalid creds"))  //if credentials don't exist 
            {
                if (!showError)
                {
                    PlayerPrefs.SetString("email", "");
                    PlayerPrefs.SetString("password", "");
                }
                inputPassword.text = "";
                wrongPassword.SetActive(showError);
            }
            else                                           //connection and credentials success
            {
                PlayerPrefs.SetString("email", email);
                PlayerPrefs.SetString("password", password);
                if (emptyEmail.activeSelf) emptyEmail.SetActive(false);
                if (emptyPassword.activeSelf) emptyPassword.SetActive(false);
                if (wrongPassword.activeSelf) wrongPassword.SetActive(false);
                user = UserInfo.Download();
                user.SetData();
                playerID = user.playerID;
                SceneManager.LoadScene("Main");
            }
        }
        else                                               //connection failure
        {
            networkError.SetActive(showError);
        }
        connecting.SetActive(false);
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
