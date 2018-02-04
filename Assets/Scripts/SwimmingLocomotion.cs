using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SwimmingLocomotion : MonoBehaviour {

    private VRTK_ControllerEvents Controller;

    private GameObject ControllerObject;

    private bool isMoving = false;
    private Transform playArea;
    private Vector3 startScaledLocalPosition;
    private Vector3 startGrabPointWorldPosition;
    private Vector3 startPlayAreaWorldOffset;

    // Use this for initialization
    void Start () {
        Controller = GetComponent<VRTK_ControllerEvents>();
        ControllerObject = gameObject;

        Controller.SubscribeToButtonAliasEvent(VRTK_ControllerEvents.ButtonAlias.GripPress, true, OnGripPress);
        Controller.SubscribeToButtonAliasEvent(VRTK_ControllerEvents.ButtonAlias.GripPress, false, OnGripRelease);

        playArea = VRTK_DeviceFinder.PlayAreaTransform();

        ControllerObject = VRTK_DeviceFinder.GetActualController(ControllerObject);
    }

    void OnGripPress(object sender, ControllerInteractionEventArgs e)
    {
         OnStartGrabEther();
    }

    void OnGripRelease(object sender, ControllerInteractionEventArgs e)
    {
        OnStopGrabEther();
    }

    protected virtual Vector3 GetScaledLocalPosition(Transform objTransform)
    {
        return playArea.localRotation * Vector3.Scale(objTransform.localPosition, playArea.localScale);
    }

    void OnStartGrabEther()
    {
        isMoving = true;
        startScaledLocalPosition = GetScaledLocalPosition(ControllerObject.transform);
        startGrabPointWorldPosition = ControllerObject.transform.position;
        startPlayAreaWorldOffset = playArea.transform.position - ControllerObject.transform.position;
    }

    void OnStopGrabEther()
    {
        isMoving = false;
    }


    // Update is called once per frame
    void Update () {
        if (isMoving)
        {
            Vector3 controllerLocalOffset = GetScaledLocalPosition(ControllerObject.transform) - startScaledLocalPosition;
            playArea.position = startGrabPointWorldPosition + startPlayAreaWorldOffset - controllerLocalOffset;
        }
    }
}
