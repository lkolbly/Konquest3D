using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapSpawner : NetworkBehaviour {

    public GameObject planetPrefab;
    //public GameObject desertGeneratorPrefab;
    public GameObject playerObject;

    // Returns a value between -1 and 1, peaking at 0.
    private float triangleDistribution()
    {
        while (true)
        {
            var x = Random.value * 2.0f - 1.0f;
            var y = Random.value;
            if (y < 1.0f - Mathf.Abs(x))
            {
                return x;
            }
        }
    }

    private string PadNumber(int n, int length)
    {
        var s = n.ToString("D");
        return new string('0', length - s.Length) + s;
    }

    // Use this for initialization
    void Start () {
        // Instantiate a bunch of planets
        //GameObject.Find("NetworkLobby").GetComponent<MyNetworkManager>();
        Debug.Log(isClient + " " + isServer);
        var networkGame = !isClient && !isServer;
        // If we're a server or a singleplayer game, generate planets
        if (networkGame && isClient)
        {
            return;
        }
        for (var i = 0; i < 10; ++i)
        {
            var planetObject = Instantiate(planetPrefab, Random.insideUnitSphere + transform.position, Quaternion.identity);
            //var generatorObject = Instantiate(desertGeneratorPrefab, Vector3.zero, Quaternion.identity);
            //generatorObject.transform.parent = planetObject.transform;
            var graphicId = (int)(Random.value * 64);
            if (!networkGame)
            {
                var prefab = Resources.Load("generated/planet" + PadNumber(graphicId, 3), typeof(GameObject)) as GameObject;
                var planetGraphics = Instantiate(prefab, planetObject.transform.position, Quaternion.identity);
                planetGraphics.transform.parent = planetObject.transform;
                planetGraphics.transform.localScale = new Vector3(1f, 1f, 1f);
            }

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

            //var planetGenerator = planetObject.GetComponent<GeneratePlanet>();

            //planetGenerator.material = planet.GetComponent<Renderer>().materials[0];

            if (isServer)
            {
                NetworkServer.Spawn(planetObject);
                planet.RpcSetGraphics(graphicId);
                //NetworkServer.Spawn(planetGraphics);
            }
        }
    }
    
    // Update is called once per frame
    void Update () {
        
    }
}
