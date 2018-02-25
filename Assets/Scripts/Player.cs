using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    private GameObject sourcePlanet;

    public GameObject shipPrefab;

    [SyncVar(hook = "OnSetTeamId")]
    public int teamId;

    private int selectedFleetSize;

    private bool hasNotifiedServer = false;

    void OnSetTeamId(int teamId)
    {
        // Go re-render all planets
        foreach (var planet in GameObject.FindObjectsOfType<Planet>())
        {
            planet.GetComponent<Planet>().UpdateTooltip();
        }

    }

    public void SetSelectedFleetSize(int size)
    {
        selectedFleetSize = size;
    }

    public bool isInSelectTargetMode()
    {
        return sourcePlanet != null;
    }

    public void setSource(GameObject source)
    {
        sourcePlanet = source;
    }

    public GameObject getSource()
    {
        return sourcePlanet;
    }

    public void setTarget(GameObject targetPlanet)
    {
        // Create some ships at the source planet headed to the target planet
        LaunchFleetDispatch(sourcePlanet, targetPlanet, selectedFleetSize);
        selectedFleetSize = 1;

        // Unselect the source planet
        sourcePlanet.GetComponent<Planet>().DisplayTeamColor();
        sourcePlanet = null;
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer() called");

        // Tell the server we're ready
        var networkManager = GameObject.Find("NetworkLobby").GetComponent<MyNetworkManager>();
        //var mapSpawner = GameObject.Find("GameManager").GetComponent<MapSpawner>();
        //mapSpawner.CmdPlayersReady();

        networkManager.localPlayerObject = gameObject;

        // Go re-render all planets
        foreach (var planet in GameObject.FindObjectsOfType<Planet>())
        {
            planet.GetComponent<Planet>().UpdateTooltip();
        }
    }

    // Use this for initialization
    void Start () {
        
    }

    [Command]
    public void CmdPlayersReady()
    {
        var mapSpawner = GameObject.Find("GameManager").GetComponent<MapSpawner>();
        mapSpawner.PlayerCheckin();
    }

    [Command]
    public void CmdPlayerLaunchFleet(GameObject source, GameObject target, int fleetSize)
    {
        source.GetComponent<Planet>().LaunchFleetDispatch(target, fleetSize);
    }

    private void LaunchFleetDispatch(GameObject source, GameObject target, int fleetSize)
    {
        if (isClient)
        {
            CmdPlayerLaunchFleet(source, target, fleetSize);
        }
        else
        {
            source.GetComponent<Planet>().LaunchFleetDispatch(target, fleetSize);
        }
    }

    // Update is called once per frame
    void Update() {
        if (!isClient || !hasAuthority)
        {
            return;
        }

        if (!hasNotifiedServer)
        {
            Debug.Log("Sending player ready notification!");
            //var playerReadyNotifier = gameObject.GetComponent<PlayerReadyNotifier>();
            //playerReadyNotifier.CmdPlayersReady();
            CmdPlayersReady();
            hasNotifiedServer = true;
            /*var playerReadyNotifierObject = GameObject.Find("PlayerReadyNotifier");
            if (playerReadyNotifierObject != null)
            {
                Debug.Log("Sending player ready notification!");
                var playerReadyNotifier = playerReadyNotifierObject.GetComponent<PlayerReadyNotifier>();
                playerReadyNotifier.CmdPlayersReady();
                hasNotifiedServer = true;
            }*/
        }

        // Useful for testing multiplayer without multiple players
        if (Random.value < 0.003 && isLocalPlayer)
        {
            Debug.Log("Launching a ship");
            
            // Launch 1 ship from one of out planets to one of theirs

            // Classify all planets: Ours, theirs, and neutral
            List<GameObject> ourPlanets = new List<GameObject>();
            List<GameObject> theirPlanets = new List<GameObject>();
            List<GameObject> neutralPlanets = new List<GameObject>();

            foreach (GameObject planetObj in Object.FindObjectsOfType<GameObject>())
            {
                var planet = planetObj.GetComponent<Planet>();
                if (planet != null)
                {
                    if (planet.teamId == teamId)
                    {
                        ourPlanets.Add(planetObj);
                    }
                    else if (planet.teamId == 0)
                    {
                        neutralPlanets.Add(planetObj);
                    }
                    else
                    {
                        theirPlanets.Add(planetObj);
                    }
                }
            }

            LaunchFleetDispatch(ourPlanets[0], neutralPlanets[0], 1);
            //ourPlanets[0].GetComponent<Planet>().LaunchFleetDispatch(neutralPlanets[0], 1);
        }
    }
}
