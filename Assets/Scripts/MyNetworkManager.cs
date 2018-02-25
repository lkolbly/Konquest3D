using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkLobbyManager {

    private int playerCount = 0;
    public GameObject localPlayerObject;

    // Use this for initialization
    void Start () {
        
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Server adding player "+playerCount+" "+playerControllerId);
        var player = Instantiate(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<Player>().teamId = ++playerCount;
        NetworkServer.AddPlayerForConnection(conn, player, (short)(playerCount-1));
        return player;
    }

    public override void OnLobbyClientEnter()
    {
        base.OnLobbyClientEnter();

        Debug.Log("Client entering lobby");
    }

    // Update is called once per frame
    void Update () {
        
    }
}
