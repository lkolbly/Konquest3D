using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    /*public void OnClick()
    {
        //var scene = SceneManager.GetSceneByName("konquest3d");
        var sceneIndex = SceneUtility.GetBuildIndexByScenePath("Assets/konquest3d.scene");
        SceneManager.LoadScene(sceneIndex); //scene.buildIndex);
    }*/

    public void GoToScene(string scenePath)
    {
        var sceneIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
        SceneManager.LoadScene(sceneIndex);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
