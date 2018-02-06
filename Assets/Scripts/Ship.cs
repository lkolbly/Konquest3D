using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Ship : NetworkBehaviour {

    public GameObject targetPlanet;
    public GameObject line;

    private int teamId;
    private float effectiveness;
    private int numShips; // Since this is actually a fleet

    public void SetStats(int teamId, int numShips, float effectiveness)
    {
        this.teamId = teamId;
        this.numShips = numShips;
        this.effectiveness = effectiveness;
    }

    // Use this for initialization
    void Start () {
        Debug.Log("Ship created");
    }

    [ClientRpc]
    void RpcDestroyShip()
    {
        Destroy(line);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // We only get called on the server, or in singleplayer
        if (Network.isClient)
        {
            return;
        }

        if (other.gameObject != targetPlanet)
        {
            return;
        }

        var otherPlanet = other.gameObject.GetComponent<Planet>();
        if (otherPlanet != null)
        {
            Debug.Log("Hit planet");

            otherPlanet.DoInvasion(teamId, numShips, effectiveness);

            // Destroy on the server
            Destroy(line);
            Destroy(gameObject);

            // Destroy on the clients
            RpcDestroyShip();
        }
    }

    // Update is called once per frame
    void Update () {
        
    }
}
