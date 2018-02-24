using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerReadyNotifier : NetworkBehaviour
{
    private int numCheckins = 0;

    [Command]
    public void CmdPlayersReady()
    {
        var mapSpawner = GameObject.Find("GameManager").GetComponent<MapSpawner>();
        mapSpawner.PlayerCheckin();
        /*numCheckins++;
        Debug.Log("Number of checkins is "+numCheckins);
        if (numCheckins == 2)
        {
            Debug.Log("Client players are ready");
            var mapSpawner = GameObject.Find("GameManager").GetComponent<MapSpawner>();
            mapSpawner.BuildWorld();
        }*/
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
