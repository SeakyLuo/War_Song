using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountCreation : MonoBehaviour {

    public InputField email, password, confirmPassword, playerName;
    public GameObject emailWarn, passwordWarn, confirmPasswordWarn, playerNameWarn, networkError, accountExists;
    public Button confirmCreation, login;
    public Toggle readAgreement;

    private bool goodEmail = false, goodPassword = false, matchPassword = false, goodPlayerName = false;

    public void GoodEmail()
    {
        if (email.text == "") return;
        // account exist or email doesn't exist
        goodEmail = email.text.Contains("@");
        emailWarn.SetActive(!goodEmail);
        confirmCreation.interactable = (goodEmail && goodPassword && matchPassword && goodPlayerName && readAgreement.isOn);
    }

    public void GoodPassword()
    {
        if (password.text == "") return;
        goodPassword = (password.text.Length >= 8);
        passwordWarn.SetActive(!goodPassword);
        confirmCreation.interactable = (goodEmail && goodPassword && matchPassword && goodPlayerName && readAgreement.isOn);
        MatchPassword();
    }

    public void MatchPassword()
    {
        if (confirmPassword.text == "") return;
        matchPassword = (password.text == confirmPassword.text);
        confirmPasswordWarn.SetActive(!matchPassword);
        confirmCreation.interactable = (goodEmail && goodPassword && matchPassword && goodPlayerName && readAgreement.isOn); ;
    }

    public void GoodPlayerName()
    {
        if (playerName.text == "") return;
        goodPlayerName = (playerName.text.Length >= 2);
        playerNameWarn.SetActive(!goodPlayerName);
        confirmCreation.interactable = (goodEmail && goodPassword && matchPassword && goodPlayerName && readAgreement.isOn); ;
    }

    public void ConfirmCreation()
    {
        StartCoroutine(Register());
    }

    public IEnumerator Register()
    {
        // create an account in the server
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("email", email.text);
        infoToPhp.AddField("password", password.text);
        infoToPhp.AddField("userName", playerName.text);

        WWW sendToPhp = new WWW("http://localhost:8888/action_reg.php", infoToPhp);
        yield return sendToPhp;
        Debug.Log(sendToPhp.error);
        if(sendToPhp.error.Contains("Cannot connect"))
        {
            networkError.SetActive(true);
        }
        else if (string.IsNullOrEmpty(sendToPhp.error))
        {
            if (sendToPhp.text.Contains("Error Could not create."))
            {
                accountExists.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetString("email", email.text);
                PlayerPrefs.SetString("password", password.text);

                CancelCreation();
                StartCoroutine(transform.parent.Find("Login").GetComponent<Login>().RequestLogin(email.text, password.text));
            }
        }
    }

    public void CloseAccountExists()
    {
        email.text = "";
        accountExists.SetActive(false);
    }

    public void Agree()
    {
        confirmCreation.interactable = (goodEmail && goodPassword && matchPassword && goodPlayerName && readAgreement.isOn);
    }

    public void CancelCreation()
    {
        email.text = password.text = confirmPassword.text = playerName.text = "";
        goodEmail =  goodPassword =  matchPassword = goodPlayerName = readAgreement.isOn = false;
        gameObject.SetActive(false);
    }
}
