using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void OnClick()
    {
        //var scene = SceneManager.GetSceneByName("konquest3d");
        SceneManager.LoadScene(1); //scene.buildIndex);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
