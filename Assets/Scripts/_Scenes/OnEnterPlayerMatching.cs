using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class OnEnterPlayerMatching : MonoBehaviour
{
    public Text rank, tipsText;
    public Button rankedMode, casualMode, launchWar, cancelMatching;
    public GameObject launchWarText, settingsPanel, matchingPanel, cancelMatchingText;
    public Transform lineups;
    public Slider slider;
    public Image randomCardImage;

    private static List<string> tips = new List<string> { "Hello", "Have fun"};

    private List<GameObject> lineupObjects;
    private List<GameObject> xs;
    private bool cancel = false;
    private int lineupSelected = -1;
    private bool matchStart = false;
    private WWWForm infoToPhp;
    private MatchInfo playerMatchInfo;

    private void Start()
    {
        rank.text = Login.user.rank.ToString();
        int lineupsCount = Login.user.lineups.Count;
        lineupObjects = new List<GameObject>();
        xs = new List<GameObject>();
        for (int i = 0; i < LineupsManager.lineupsLimit; i++)
        {
            lineupObjects.Add(lineups.transform.Find("Lineup" + i.ToString()).gameObject);
            xs.Add(lineupObjects[i].transform.Find("Unavailable").gameObject);
            if (i < lineupsCount)
            {
                lineupObjects[i].GetComponentInChildren<Text>().text = Login.user.lineups[i].lineupName;
                lineupObjects[i].transform.Find("ImagePanel/Image").GetComponent<Image>().sprite = Database.FindPieceAttributes(Login.user.lineups[i].GetGeneral()).image;
                lineupObjects[i].GetComponent<Button>().interactable = Login.user.lineups[i].IsComplete();
                xs[i].SetActive(!Login.user.lineups[i].IsComplete());
            }
            else lineupObjects[i].SetActive(false);
        }
        if (Login.user.lastLineupSelected == -1)
        {
            launchWarText.SetActive(false);
            launchWar.interactable = false;
        }
        else SelectLineup(Login.user.lastLineupSelected);
        switch (Login.user.lastModeSelected)
        {
            case "":
                RankedMode();
                break;
            case "Ranked Mode":
                RankedMode();
                break;
            case "Casual Mode":
                CasualMode();
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            settingsPanel.SetActive(true);
        if (!matchStart || cancel) return;
        WWW sendToPhp = new WWW("http://47.151.234.225/returnUserMatchInfo.php", infoToPhp);
        while (!sendToPhp.isDone) { }
        if (sendToPhp.text != "" && !sendToPhp.text.Contains("Warning"))
        {
            matchStart = false;
            CancelInteractable(false);
            // Return Enemy MatchInfo
            MatchInfo enemyMatchInfo = MatchInfo.ToClass(sendToPhp.text);
            WWWForm order = new WWWForm();
            order.AddField("playerID", Login.playerID);
            WWW getOrder = new WWW("http://47.151.234.225/returnMatchOrder.php", order);
            while (!getOrder.isDone) { }
            OnEnterGame.gameInfo = new GameInfo(Login.user.lastModeSelected, playerMatchInfo, enemyMatchInfo, int.Parse(getOrder.text));
            StopAllCoroutines();
            matchingPanel.SetActive(false);
            LaunchWar();
        }
    }

    public void Match()
    {
        ChangeTips();
        CancelInteractable(true);
        matchingPanel.SetActive(true);
        StartCoroutine(ShowProgress());
        Login.user.SetLastLineupSelected(lineupSelected);
        playerMatchInfo = new MatchInfo(Login.user, Login.user.lineups[Login.user.lastLineupSelected]);
        matchStart = true;
        infoToPhp = new WWWForm();
        infoToPhp.AddField("mode", Login.user.lastModeSelected);
        infoToPhp.AddField("boardName", Login.user.lineups[Login.user.lastLineupSelected].boardName);
        infoToPhp.AddField("playerID", Login.playerID);
        infoToPhp.AddField("matchInfo", playerMatchInfo.ToJson());
        WWW sendToPhp = new WWW("http://47.151.234.225/uploadMatchUsers.php", infoToPhp);
        while (!sendToPhp.isDone) { }
    }

    public void CancelMatching()
    {
        cancel = true;
        matchStart = false;
        // Cancel network matching
        WWWForm deleteMatchInfo = new WWWForm();
        deleteMatchInfo.AddField("playerID", Login.playerID);
        WWW sendToPhp = new WWW("http://47.151.234.225/deleteMatchUsers.php", infoToPhp);
        while (!sendToPhp.isDone) { }
        matchingPanel.SetActive(false);
        StopAllCoroutines();
    }

    private IEnumerator ShowProgress()
    {
        slider.value = 0;
        bool flip = false;
        float increment = 0.02f;
        while (true)
        {
            if (slider.value == 1) flip = true;
            else if (slider.value == 0)
            {
                flip = false;
                randomCardImage.sprite = Database.RandomImage();
            }
            if (flip) slider.value -= increment;
            else slider.value += increment;
            randomCardImage.fillAmount = slider.value;
            yield return new WaitForSeconds(increment);
        }
    }

    private void ChangeTips()
    {
        tipsText.text = tips[Random.Range(0, tips.Count)];
    }

    private void CancelInteractable(bool interactable)
    {
        cancelMatching.interactable = interactable;
        cancelMatchingText.SetActive(interactable);
    }

    private void LaunchWar()
    {
        SceneManager.LoadScene("GameMode");
    }

    public void Back()
    {
        SceneManager.LoadScene("Main");
    }

    public void EnterCollection()
    {
        SwitchScenes.switchSceneCaller = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Collection");
    }

    public void RankedMode()
    {
        Login.user.lastModeSelected = "Ranked Mode";
        rankedMode.GetComponent<Image>().sprite = rankedMode.spriteState.pressedSprite;
        casualMode.GetComponent<Image>().sprite = casualMode.spriteState.disabledSprite;
    }

    public void CasualMode()
    {
        Login.user.lastModeSelected = "Casual Mode";
        rankedMode.GetComponent<Image>().sprite = rankedMode.spriteState.disabledSprite;
        casualMode.GetComponent<Image>().sprite = casualMode.spriteState.pressedSprite;
    }

    public void SelectLineup(int number)
    {
        lineupSelected = number;
        if (!launchWar.interactable)
        {
            launchWarText.SetActive(true);
            launchWar.interactable = true;
        }
        if (Login.user.lastLineupSelected != -1 && number != Login.user.lastLineupSelected)
            lineupObjects[Login.user.lastLineupSelected].GetComponent<Image>().sprite = lineupObjects[Login.user.lastLineupSelected].GetComponent<Button>().spriteState.disabledSprite;
        lineupObjects[number].GetComponent<Image>().sprite = lineupObjects[number].GetComponent<Button>().spriteState.highlightedSprite;
    }
}
