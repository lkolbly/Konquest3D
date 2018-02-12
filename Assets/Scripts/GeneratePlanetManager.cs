using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePlanetManager : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        // Tell the generator about us
        GetComponentInChildren<GeneratePlanet>().material = GetComponent<Renderer>().materials[0];
        Debug.Log(GetComponent<Renderer>().materials[0]);
        GetComponentInChildren<GeneratePlanet>().Generate();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
