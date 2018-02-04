﻿using System.Collections;
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
        Debug.Log("Server adding player");
        var player = Instantiate(gamePlayerPrefab, Vector3.zero, Quaternion.identity);
        player.GetComponent<Player>().teamId = ++playerCount;
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        return player;
    }

    // Update is called once per frame
    void Update () {
		
	}
}