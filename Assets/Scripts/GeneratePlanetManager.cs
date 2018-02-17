using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePlanetManager : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        // Tell the generator about us
        /*GetComponentInChildren<GeneratePlanet>().material = GetComponent<Renderer>().materials[0];
        Debug.Log(GetComponent<Renderer>().materials[0]);
        GetComponentInChildren<GeneratePlanet>().Generate();*/
        //var prefab = Resources.Load("Assets/Resources/generated/planet000.prefab") as GameObject;
        //prefab.transform.parent = gameObject.transform;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
