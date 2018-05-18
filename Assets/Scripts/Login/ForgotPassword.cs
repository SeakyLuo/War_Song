using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgotPassword : MonoBehaviour {

    public GameObject resetPasswordPanel;
    public GameObject newPasswordWarn, confirmPasswordWarn;
    public InputField newPassword, confirmPassword;
    public Button sendAgain, confirmAndLogin;
    public Text countdown;

    private bool goodPassword = false, matchPassword = false;

    private void OnEnable()
    {
        SendAgain();
    }

    private IEnumerator Timer()
    {
        int seconds = 60;
        while (seconds >= 0)
        {
            countdown.text = (seconds--).ToString() + "s";
            yield return new WaitForSeconds(1.0f);
        }
        countdown.gameObject.SetActive(false);
        sendAgain.interactable = true;
    }

    public void Cancel()
    {
        gameObject.SetActive(false);
    }

    public void Verify()
    {
        //To frequent check?
        //if not match report unmatch and return
        resetPasswordPanel.SetActive(true);
    }

    public void SendAgain()
    {
        sendAgain.interactable = false;
        countdown.gameObject.SetActive(true);
        StartCoroutine(Timer());
    }


    public void GoodPassword()
    {
        if (newPassword.text == "") return;
        goodPassword = (newPassword.text.Length >= 8);
        newPasswordWarn.SetActive(!goodPassword);
        confirmAndLogin.interactable = (goodPassword && matchPassword);
        MatchPassword();
    }

    public void MatchPassword()
    {
        if (confirmPassword.text == "") return;
        matchPassword = (newPassword.text == confirmPassword.text);
        confirmPasswordWarn.SetActive(!matchPassword);
        confirmAndLogin.interactable = (goodPassword && matchPassword); ;
    }

    public void ConfirmAndLogin()
    {
        PlayerPrefs.SetString("password", newPassword.text);
        // pass the value to the server
        newPassword.text = confirmPassword.text = "";
        resetPasswordPanel.SetActive(false);
        gameObject.SetActive(false);
        transform.parent.GetComponent<Login>().RequestLogin(PlayerPrefs.GetString("password"), newPassword.text);
    }
}
