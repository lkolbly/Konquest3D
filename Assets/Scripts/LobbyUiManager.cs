using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        networkManager.StartServer();

        SwitchToMatchLobbyUi();
    }

    public void OnConnectToServerClicked()
    {
        networkManager.networkAddress = "pillow.rscheme.org";
        networkManager.networkPort = 3058;
        networkManager.StartClient();

        SwitchToMatchLobbyUi();
    }

    public void OnReadyClicked()
    {
        //
    }

    public void OnLeaveMatchClicked()
    {
        //
    }

    private void SwitchToMatchLobbyUi()
    {
        mainCanvas.gameObject.SetActive(false);
        matchCanvas.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
