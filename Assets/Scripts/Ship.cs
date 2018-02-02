using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

    public GameObject targetPlanet;
    public GameObject line;

    private int teamId;
    private float effectiveness;
    private int numShips; // Since this is actually a fleet

    public void SetStats(int teamId, int numShips, float effectiveness)
    {
        this.teamId = teamId;
        this.numShips = numShips;
        this.effectiveness = effectiveness;
    }

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        /*var damageableComponent = other.gameObject.GetComponent<Damageable>();
        if (damageableComponent != null)
        {
            damageableComponent.DealDamage(1.0f);
        }*/

        if (other.gameObject != targetPlanet)
        {
            return;
        }

        var otherPlanet = other.gameObject.GetComponent<Planet>();
        if (otherPlanet != null)
        {
            Debug.Log("Hit planet");

            otherPlanet.DoInvasion(teamId, numShips, effectiveness);

            Destroy(line);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
