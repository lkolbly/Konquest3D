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

        // If we can't find any neutral planets, attack a player. Otherwise give up.
        if (neutralPlanets.Count == 0)
        {
            if (theirPlanets.Count == 0)
            {
                return;
            }

            neutralPlanets = theirPlanets; // We attack the neutral planets... So pretend theirs are neutral.
        }

        foreach (GameObject planetObj in ourPlanets)
        {
            var planet = planetObj.GetComponent<Planet>();
            if (planet.GetNumberOfShips() > 1)
            {
                planet.LaunchFleetDispatch(neutralPlanets[0], 1);
            }
        }
    }
}
