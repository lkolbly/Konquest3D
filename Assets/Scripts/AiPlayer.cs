using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : MonoBehaviour {

    public int teamId;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
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

        // If we have >1 ship, send to a neutral planet
        if (neutralPlanets.Count == 0)
        {
            return;
        }

        foreach (GameObject planetObj in ourPlanets)
        {
            var planet = planetObj.GetComponent<Planet>();
            if (planet.GetNumberOfShips() > 1)
            {
                var fleet = planet.LaunchFleet(neutralPlanets[0], 1);

                //fleet.GetComponent<Rigidbody>().velocity = neutralPlanets[0].transform.position - planetObj.transform.position;
                //fleet.GetComponent<Ship>().targetPlanet = neutralPlanets[0];
            }
        }
	}
}
