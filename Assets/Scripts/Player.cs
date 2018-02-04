﻿using System.Collections;
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
        var ships = sourcePlanet.GetComponent<Planet>().LaunchFleet(targetPlanet, selectedFleetSize);
        selectedFleetSize = 1;

        // Unselect the source planet
        sourcePlanet.GetComponent<Planet>().DisplayTeamColor();
        sourcePlanet = null;
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer() called");
        GameObject.Find("NetworkLobby").GetComponent<MyNetworkManager>().localPlayerObject = gameObject;

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
        
    }
}
