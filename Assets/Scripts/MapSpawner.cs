using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour {

    public GameObject planetPrefab;

	// Use this for initialization
	void Start () {
        // Instantiate a bunch of planets
        for (var i = 0; i < 10; ++i)
        {
            var planetObject = Instantiate(planetPrefab, Random.insideUnitSphere, Quaternion.identity);
            var planet = planetObject.GetComponent<Planet>();

            planet.constructionTime = 0.0f;
            for (var j = 0; j < 10; ++j)
            {
                planet.constructionTime += Random.value;
            }

            planet.shipEffectiveness = (Random.value + Random.value) / 2.0f;
            if (i == 0)
            {
                planet.teamId = 1;
                planet.constructionTime = 5.0f;
                planet.shipEffectiveness = 0.75f;
            }
            else if (i == 1)
            {
                planet.teamId = 2;
                planet.constructionTime = 5.0f;
                planet.shipEffectiveness = 0.75f;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
