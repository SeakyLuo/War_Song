using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AccountCreation : MonoBehaviour {

    public InputField email, password, confirmPassword, playerName;
    public GameObject emailWarn, passwordWarn, confirmPasswordWarn, playerNameWarn, networkError, accountExists;
    public Button confirmCreation;
    public Toggle readAgreement;
    public Login login;

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
        goodPlayerName = 2 <= playerName.text.Length && playerName.text.Length <= 24;
        playerNameWarn.SetActive(!goodPlayerName);
        confirmCreation.interactable = (goodEmail && goodPassword && matchPassword && goodPlayerName && readAgreement.isOn); ;
    }

    public void ConfirmCreation()
    {
        // create an account in the server
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("email", email.text);
        infoToPhp.AddField("password", password.text);
        infoToPhp.AddField("userName", playerName.text);
        infoToPhp.AddField("userJson", UserInfo.ClassToJson(new UserInfo(playerName.text, GeneratePlayerID()))); //new CheatAccount();

        WWW sendToPhp = new WWW("http://47.151.234.225/action_reg.php", infoToPhp);
        while (!sendToPhp.isDone) { }

        if (string.IsNullOrEmpty(sendToPhp.error))
        {
            if (sendToPhp.text.Contains("User Exists"))
            {
                email.text = "";
                accountExists.SetActive(true);
            }
            else if (sendToPhp.text.Contains("Error Could not create."))
            {
                networkError.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetString("email", email.text);
                PlayerPrefs.SetString("password", password.text);

                login.RequestLogin(email.text, password.text);
                CancelCreation();
            }
        }
        else if (sendToPhp.error.Contains("Cannot connect"))
        {
            networkError.SetActive(true);
        }
    }

    public int GeneratePlayerID()
    {
        return 10000000;
        WWWForm infoToPhp = new WWWForm();
        infoToPhp.AddField("playerID", "playerID");
        WWW sendToPhp = new WWW("http://47.151.234.225/action_reg.php", infoToPhp);
        while (!sendToPhp.isDone) { }
        return int.Parse(sendToPhp.text) + 10000000;        
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
