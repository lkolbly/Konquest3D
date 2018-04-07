using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyUiManager : MonoBehaviour {

    public Canvas mainCanvas;
    public Canvas matchCanvas;
    public GameObject readyButtonText;
    public MyNetworkManager networkManager;
    public InputField input;

	// Use this for initialization
	void Start () {
        matchCanvas.gameObject.SetActive(false);
	}

    public void OnReturnToMenuClicked()
    {
        var sceneIndex = SceneUtility.GetBuildIndexByScenePath("Assets/mainMenu.unity");
        SceneManager.LoadScene(sceneIndex);
    }

    public void OnStartServerClicked()
    {
        networkManager.StartHost();

        SwitchToMatchLobbyUi();
    }

    public void OnConnectToServerClicked()
    {
        networkManager.networkAddress = input.text;
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
            readyButtonText.GetComponent<Text>().text = "Ready";
        }
        else
        {
            networkPlayer.SendReadyToBeginMessage();
            readyButtonText.GetComponent<Text>().text = "Unready";
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //SwitchToMainLobbyUi();
    }

    private NetworkLobbyPlayer GetLocalNetworkPlayer()
    {
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            var nm = go.GetComponent<NetworkLobbyPlayer>();
            if (nm != null)
            {
                if (nm.isLocalPlayer)
                {
                    return nm;
                }
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
