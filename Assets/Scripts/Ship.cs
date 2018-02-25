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
    }

    [Command]
    void CmdInvade(GameObject target)
    {
        target.GetComponent<Planet>().DoInvasion(teamId, numShips, effectiveness);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ship hit planet: "+isClient+" "+isServer);
        // We only get called on the server, or in singleplayer
        if (!hasAuthority)
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
            CmdInvade(other.gameObject);

            Destroy(line);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
        
    }
}
