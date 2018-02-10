using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class Controller : MonoBehaviour {

    private int lastSelectedFleetSize;
    public Text touchpadFleetCount;
    public GameObject playerObject;

    // Use this for initialization
    void Start () {
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
        GetComponent<VRTK_ControllerEvents>().TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
    }

    private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
    {
        //
    }

    private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        //
    }

    private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        var networkManager = GameObject.Find("NetworkLobby");
        var localPlayerObject = networkManager == null ?
            playerObject :
            networkManager.GetComponent<MyNetworkManager>().localPlayerObject;
        var player = localPlayerObject.GetComponent<Player>();
        if (player.getSource() == null)
        {
            return;
        }
        var sourcePlanet = player.getSource().GetComponent<Planet>();
        var maxShips = sourcePlanet.GetNumberOfShips();
        var selectedFleetSize = (int)Mathf.Floor(e.touchpadAngle * maxShips / 360.0f);
        Debug.Log(selectedFleetSize);
        if (lastSelectedFleetSize != selectedFleetSize)
        {
            VRTK_ControllerHaptics.TriggerHapticPulse(e.controllerReference, 0.4f, 0.01f, 0.01f);
            touchpadFleetCount.text = selectedFleetSize.ToString();
        }
        lastSelectedFleetSize = selectedFleetSize;
        player.SetSelectedFleetSize(selectedFleetSize);
    }

    private void DoTriggerPressed(object sender, ControllerInteractionEventArgs e)
    {
        // Find the first planet we're touching
        foreach (GameObject planetObj in Object.FindObjectsOfType<GameObject>())
        {
            var planet = planetObj.GetComponent<Planet>();
            if (planet != null)
            {
                if (planet.TrySelecting())
                {
                    Debug.Log("Found a planet to select");
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        
    }
}
