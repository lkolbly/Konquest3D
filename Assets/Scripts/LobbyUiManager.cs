﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyUiManager : MonoBehaviour {

    public Canvas mainCanvas;
    public Canvas matchCanvas;
    public MyNetworkManager networkManager;

	// Use this for initialization
	void Start () {
        matchCanvas.gameObject.SetActive(false);
	}

    public void OnStartServerClicked()
    {
        networkManager.StartHost();

        SwitchToMatchLobbyUi();
    }

    public void OnConnectToServerClicked()
    {
        networkManager.networkAddress = "localhost";
        networkManager.networkPort = 3058;
        networkManager.StartClient();

        SwitchToMatchLobbyUi();
    }

    public void OnReadyClicked()
    {
        Debug.Log("Player ready");
        var networkPlayer = GetLocalNetworkPlayer();
        if (networkPlayer.readyToBegin)
        {
            networkPlayer.SendNotReadyToBeginMessage();
        }
        else
        {
            networkPlayer.SendReadyToBeginMessage();
        }
    }

    public void OnLeaveMatchClicked()
    {
        Debug.Log("Leaving match");
        if (!Network.isClient)
        {
            // We're the host
            networkManager.StopHost();
        }
        else
        {
            // We're the client
            networkManager.StopClient();
        }
        SwitchToMainLobbyUi();
    }

    private NetworkLobbyPlayer GetLocalNetworkPlayer()
    {
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            var nm = go.GetComponent<NetworkLobbyPlayer>();
            if (nm != null)
            {
                return nm;
            }
        }
        return null;
    }

    private void SwitchToMatchLobbyUi()
    {
        mainCanvas.gameObject.SetActive(false);
        matchCanvas.gameObject.SetActive(true);
    }

    private void SwitchToMainLobbyUi()
    {
        mainCanvas.gameObject.SetActive(true);
        matchCanvas.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log(networkManager.numPlayers);
	}
}
