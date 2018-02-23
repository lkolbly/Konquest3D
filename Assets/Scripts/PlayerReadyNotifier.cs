using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerReadyNotifier : NetworkBehaviour
{
    [Command]
    public void CmdPlayersReady()
    {
        Debug.Log("Client players are ready");
        var mapSpawner = GameObject.Find("GameManager").GetComponent<MapSpawner>();
        mapSpawner.BuildWorld();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
