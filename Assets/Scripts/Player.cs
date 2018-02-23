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
        sourcePlanet.GetComponent<Planet>().LaunchFleetDispatch(targetPlanet, selectedFleetSize);
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
        var playerReadyNotifier = GameObject.Find("PlayerReadyNotifier").GetComponent<PlayerReadyNotifier>();
        playerReadyNotifier.CmdPlayersReady();

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
    
    // Update is called once per frame
    void Update () {
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

            ourPlanets[0].GetComponent<Planet>().LaunchFleetDispatch(neutralPlanets[0], 1);
        }
    }
}
