using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private GameObject sourcePlanet;

    public GameObject shipPrefab;
    public int teamId;

    private int selectedFleetSize;

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
        //var ships = Instantiate(shipPrefab, sourcePlanet.transform.position, Quaternion.identity);
        var ships = sourcePlanet.GetComponent<Planet>().LaunchFleet(targetPlanet, selectedFleetSize);
        selectedFleetSize = 1;
        //ships.GetComponent<Rigidbody>().velocity = targetPlanet.transform.position - sourcePlanet.transform.position;
        //ships.GetComponent<Ship>().targetPlanet = targetPlanet;


        // Draw a line from source to dest
        /*var lineObject = Instantiate(linePrefab);
        var line = lineObject.GetComponent<LineRenderer>();
        line.SetPosition(0, sourcePlanet.transform.position);
        line.SetPosition(1, targetPlanet.transform.position);*/

        // Unselect the source planet
        sourcePlanet.GetComponent<Planet>().DisplayTeamColor();
        sourcePlanet = null;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
