using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections;

public class OnEnterPlayerMatching : MonoBehaviour
{
    public Text rank, tipsText;
    public Button rankedMode, casualMode, launchWar, cancelMatching;
    public GameObject launchWarText, settingsPanel, matchingPanel, cancelMatchingText;
    public Transform lineups;
    public Slider slider;

    private static List<string> tips = new List<string> { "Hello", "Have fun"};

    private List<GameObject> lineupObjects;
    private List<GameObject> xs;
    private bool cancel = false;

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
                lineupObjects[i].transform.Find("ImagePanel/Image").GetComponent<Image>().sprite = Database.FindPieceAttributes(Login.user.lineups[i].general).image;
                lineupObjects[i].GetComponent<Button>().interactable = Login.user.lineups[i].complete;
                xs[i].SetActive(!Login.user.lineups[i].complete);
            }
            else lineupObjects[i].SetActive(false);
        }
        if (Login.user.lastLineupSelected == -1)
        {
            launchWarText.SetActive(false);
            launchWar.interactable = false;
        }
        SelectLineup(Login.user.lastLineupSelected);
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

    public void Match()
    {
        // Upload Lineup Info to the server and match according to the board.
        ChangeTips();
        CancelInteractable(true);
        matchingPanel.SetActive(true);
        StartCoroutine(ShowProgress());
        Lineup lineup = Login.user.lineups[Login.user.lastLineupSelected];

        //WWWForm infoToPhp = new WWWForm();
        // Match by mode, boardName, (rank [less important])
        //infoToPhp.AddField("mode", Login.user.lastModeSelected);
        //infoToPhp.AddField("boardName", Login.user.lineups[Login.user.lastLineupSelected].boardName);
        // Return Enemy username, rank and lineup
        //infoToPhp.AddField("playerName", Login.user.username);
        //infoToPhp.AddField("rank", Login.user.rank);
        //infoToPhp.AddField("lineup", JsonUtility.ToJson(lineup));

        //infoToPhp.AddField("playerID", Login.playerID);

        //WWW sendToPhp = new WWW("http://47.151.234.225/match.php", infoToPhp);

        //while (!sendToPhp.isDone)
        //{
        //    if (cancel)
        //    {
        //        cancel = false;
        //        return;
        //    }
        //}
        //CancelInteractable(false);
        //OnEnterGame.gameInfo = GameInfo.JsonToClass(sendToPhp.text);
        StopAllCoroutines();

        matchingPanel.SetActive(false);
        LaunchWar();
    }

    //public void Connect()
    //{
    //    SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
    //    connectArgs.UserToken = this.clientSocket;   //关联用户的Socket对象
    //    connectArgs.RemoteEndPoint = this.hostEndPoint;
    //    connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);   //注册完成事件
    //    clientSocket.ConnectAsync(connectArgs);
    //    autoConnectEvent.WaitOne();   //等待连接结果

    //    SocketError errorCode = connectArgs.SocketError;
    //    if (errorCode != SocketError.Success)
    //    {
    //        throw new SocketException((int)errorCode);
    //    }
    //}

    public void CancelMatching()
    {
        cancel = true;
        // cancel network matching
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
            if (flip) slider.value -= increment;
            else slider.value += increment;
            if (slider.value == 1) flip = true;
            else if (slider.value == 0) flip = false;
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

    public void SelectLineup(int number)
    {        
        if(number != -1)
        {
            if (!launchWar.interactable)
            {
                launchWarText.SetActive(true);
                launchWar.interactable = true;
            }
            lineupObjects[number].GetComponent<Image>().sprite = lineupObjects[number].GetComponent<Button>().spriteState.highlightedSprite;
            if(Login.user.lastLineupSelected != -1)
                lineupObjects[Login.user.lastLineupSelected].GetComponent<Image>().sprite = lineupObjects[Login.user.lastLineupSelected].GetComponent<Button>().spriteState.disabledSprite;
        }
        Login.user.SetLastLineupSelected(number);
    }
}
