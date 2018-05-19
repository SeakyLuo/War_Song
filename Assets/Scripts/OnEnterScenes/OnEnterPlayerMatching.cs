using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Net.Sockets;

public class OnEnterPlayerMatching : MonoBehaviour
{
    public Text rank;
    public Button rankedMode,casualMode,launchWar;
    public GameObject launchWarText, settingsPanel, matchingPanel;
    public GameObject[] lineupObjects = new GameObject[LineupsManager.lineupsLimit];

    private GameObject[] xs = new GameObject[LineupsManager.lineupsLimit];
    private bool cancel = false;

    private void Start()
    {
        rank.text = Login.user.rank.ToString();
        int lineupsCount = Login.user.lineups.Count;
        for (int i = 0; i < LineupsManager.lineupsLimit; i++)
        {
            xs[i] = lineupObjects[i].transform.Find("Unavailable").gameObject;
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
        else
        {
            lineupObjects[Login.user.lastLineupSelected].GetComponent<Button>().Select();
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
        // Upload Lineup Info to the server and match according to the board
        matchingPanel.SetActive(true);
        //Lineup lineup = Login.user.lineups[Login.user.lastLineupSelected];

        //WWWForm infoToPhp = new WWWForm();
        //infoToPhp.AddField("mode", Login.user.lastModeSelected);
        //infoToPhp.AddField("boardName", Login.user.lineups[Login.user.lastLineupSelected].boardName);
        //infoToPhp.AddField("rank", Login.user.rank);
        //infoToPhp.AddField("playerID", Login.playerID);
        //infoToPhp.AddField("lineup", JsonUtility.ToJson(lineup));

        //WWW sendToPhp = new WWW("http://47.151.234.225/match.php", infoToPhp);

        //while (!sendToPhp.isDone)
        //{
        //    if (cancel)
        //    {
        //        cancel = false;
        //        return;
        //    }
        //}
        //OnEnterGame.gameInfo = GameInfo.JsonToClass(sendToPhp.text);

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

    public bool CancelMatching()
    {
        cancel = true;
        // cancel network matching
        matchingPanel.SetActive(false);
        return false;
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
