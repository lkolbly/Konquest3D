using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SwimmingLocomotion : MonoBehaviour {

    /*public SteamVR_TrackedObject controllerObj1;
    public SteamVR_TrackedObject controllerObj2;

    private SteamVR_Controller.Device Controller1
    {
        get { return SteamVR_Controller.Input((int)controllerObj1.index); }
    }
    private SteamVR_Controller.Device Controller2
    {
        get { return SteamVR_Controller.Input((int)controllerObj2.index); }
    }*/

    private VRTK_ControllerEvents Controller;

    private GameObject ControllerObject;

    private bool isMoving = false;
    private Transform playArea;
    private Vector3 startScaledLocalPosition;
    private Vector3 startGrabPointWorldPosition;
    private Vector3 startPlayAreaWorldOffset;
    private Quaternion controllerLastRotation;

    //private Vector3 startLocation;
    //private Vector3 startPlayAreaWorldPosition;

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
        return playArea.localRotation * Vector3.Scale(objTransform.localPosition, playArea.localScale);// playArea.localRotation * objTransform.localPosition;
        //return playArea.localRotation * Vector3.Scale(objTransform.localPosition, playArea.localScale);
    }

    /*private Vector3 GetAverageControllerScaledLocalPosition()
    {
        return (GetScaledLocalPosition(Controller1Object.transform) + GetScaledLocalPosition(Controller2Object.transform)) / 2;
    }*/

    void OnStartGrabEther()
    {
        isMoving = true;
        startScaledLocalPosition = GetScaledLocalPosition(ControllerObject.transform);
        startGrabPointWorldPosition = ControllerObject.transform.position;
        startPlayAreaWorldOffset = playArea.transform.position - ControllerObject.transform.position;
        controllerLastRotation = ControllerObject.transform.rotation;
        //startLocation = GetAverageControllerScaledLocalPosition();// Controller1.transform.position;
        //startPlayAreaWorldPosition = playArea.transform.position;// Controller1.transform.position;
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
            //Debug.Log("Moving");

            /*Vector3 lastRotationVec = controllerLastRotation * Vector3.forward;
            Vector3 currentObectRotationVec = Controller1Object.transform.rotation * Vector3.forward;
            Vector3 axis = Vector3.Cross(lastRotationVec, currentObectRotationVec);
            float angle = -Vector3.Angle(lastRotationVec, currentObectRotationVec);

            playArea.RotateAround(startGrabPointWorldPosition, axis, angle);
            controllerLastRotation = Controller1Object.transform.rotation;*/


            //Vector3 offset = GetAverageControllerScaledLocalPosition() - startLocation;// Controller1.transform.position - startLocation;
            //playArea.position = startPlayAreaWorldPosition - offset;
            //Debug.Log(GetAverageControllerScaledLocalPosition()+" "+startLocation+" "+startPlayAreaWorldPosition+" "+playArea.position);
        }
        //Debug.Log(Controller1.transform.position+" "+Controller2.transform.position);
    }
}
