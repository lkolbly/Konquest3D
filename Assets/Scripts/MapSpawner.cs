using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapSpawner : MonoBehaviour {

    public GameObject planetPrefab;
    public GameObject playerObject;

    // Returns a value between -1 and 1, peaking at 0.
    private float triangleDistribution()
    {
        while (true)
        {
            var x = Random.value;
            var y = Random.value;
            if (y < 1.0f - Mathf.Abs(x))
            {
                return x;
            }
        }
    }

    // Use this for initialization
    void Start () {
        // Instantiate a bunch of planets
        Debug.Log(Network.isClient + " " + Network.isServer);
        for (var i = 0; i < 10; ++i)
        {
            var planetObject = Instantiate(planetPrefab, Random.insideUnitSphere + transform.position, Quaternion.identity);
            var planet = planetObject.GetComponent<Planet>();

            planet.constructionTime = triangleDistribution() * 5.0f + 5.0f;
            Debug.Log(planet.constructionTime);

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

            planet.playerObject = playerObject;

            var planetGenerator = planetObject.GetComponent<GeneratePlanet>();

            planetGenerator.material = planet.GetComponent<Renderer>().materials[0];

            if (Network.isServer)
            {
                NetworkServer.Spawn(planetObject);
            }
        }
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
