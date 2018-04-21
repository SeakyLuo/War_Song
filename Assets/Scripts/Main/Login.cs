using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour {

    public InputField inputEmail, inputPassword;
    public GameObject createAccountPanel;

    private string email, password;

	// Use this for initialization
	void Start () {
        // If already has an account saved
        email = PlayerPrefs.GetString("email");
        password = PlayerPrefs.GetString("password");
        if (email != "" && password != "") login();
	}
	
	public void ConfirmLogin()
    {
        email = inputEmail.text;
        password = inputPassword.text;
        login();
    }

    public void login()
    {
        // Connect to the server
        // if not work return
        // else save email and password
        // download data
        // Info Loader
        SceneManager.LoadScene("Main");
    }

    public void CreateAccount()
    {
        createAccountPanel.SetActive(true);
    }

    public void ConfirmCreation()
    {
        // save email and password and login
    }

    public void CancelCreation()
    {
        createAccountPanel.SetActive(false);
    }
}
