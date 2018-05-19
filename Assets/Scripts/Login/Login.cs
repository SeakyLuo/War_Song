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
    public Canvas parentCanvas;

    private Database database;
    private Rect rect;

    // better to support phone number registration

    void Start () {
        rect = settingsPanel.transform.Find("MainSettings").GetComponent<RectTransform>().rect;
        // If already has an account saved
        database = new Database();
        database.Init();
        string email = PlayerPrefs.GetString("email"),
               password = PlayerPrefs.GetString("password");
        if (email != "" && password != "")
            RequestLogin(email, password, false);
	}

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            settingsPanel.SetActive(true);
        else if (Input.GetMouseButtonDown(0) && settingsPanel.activeSelf)
        {
            Vector3 mousePosition = AdjustedMousePosition();
            if (mousePosition.x < rect.x || mousePosition.x > -rect.x || mousePosition.y < rect.y || mousePosition.y > -rect.y)
                settingsPanel.SetActive(false);
        }
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
        connecting.SetActive(false);

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
    }

    private IEnumerator ChangeConnectingDots()
    {
        int count = 0;
        while (true)
        {
            count = count % 6 + 1;
            connectingDots.text = "Connecting" + new string('.', count);
            yield return new WaitForSeconds(0.5f);

        }
    }

    private Vector2 AdjustedMousePosition()
    {
        Vector2 mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, Input.mousePosition, parentCanvas.worldCamera, out mousePosition);
        return mousePosition;
    }
}
