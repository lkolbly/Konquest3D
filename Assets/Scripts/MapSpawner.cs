using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapSpawner : NetworkBehaviour {

    public GameObject planetPrefab;
    public GameObject playerObject;

    // Note: This isn't really the domain of the map spawner
    private int playerCheckinCounter = 0;

    public void PlayerCheckin()
    {
        playerCheckinCounter++;
        Debug.Log("Player checked in, " + playerCheckinCounter + " checkins");
        if (playerCheckinCounter == 2)
        {
            BuildWorld();
        }
    }

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
    public void BuildWorld() {
        // Instantiate a bunch of planets
        for (var i = 0; i < 10; ++i)
        {
            var planetObject = Instantiate(planetPrefab, Random.insideUnitSphere + transform.position, Quaternion.identity);
            var graphicId = (int)(Random.value * 64);
            if (!isClient && !isServer)
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

            if (isServer)
            {
                NetworkServer.Spawn(planetObject);
                planet.RpcSetGraphics(graphicId);
            }
        }
    }

    private void Start()
    {
        var networkGame = isClient || isServer;
        if (!networkGame)
        {
            BuildWorld();
        }
    }

    // Update is called once per frame
    void Update () {
    }
}
