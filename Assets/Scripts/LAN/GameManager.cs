using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance;
    public GameObject mainMenu;
    public GameObject serverMenu;
    public GameObject connectMemu;
    public GameObject serverPrefab;
    public GameObject clientPrefab;
    public InputField nameInput;


    private void Start()
    {
        Instance = this;
        serverMenu.SetActive(false);
        connectMemu.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void ConnectButton()
    {
        mainMenu.SetActive(false);
        connectMemu.SetActive(true);
    }
    public void HostButton()
    {
        try
        {
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = nameInput.text;
            if (c.clientName == "")
                c.clientName = "Host";
            c.ConnectToServer("127.0.0.1", 6681);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
        
        mainMenu.SetActive(false);
        serverMenu.SetActive(true);
    }
    public void ConnectToServerButton()
    {
        string hostAddress = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (hostAddress == "")
            hostAddress = "127.0.0.1";

        try
        {
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = nameInput.text;
            if (c.clientName == "")
                c.clientName = "Client";
            c.ConnectToServer(hostAddress, 6681);
            connectMemu.SetActive(false);
        }
        catch(Exception e)
        {
            Debug.Log (e.Message); 
        }
    }
    public void BackButton()
    {
        mainMenu.SetActive(true);
        serverMenu.SetActive(false);
        connectMemu.SetActive(false);

        Server s = FindObjectOfType<Server>();
        if (s != null)
            Destroy(s.gameObject);
        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(s.gameObject);
    }
}
