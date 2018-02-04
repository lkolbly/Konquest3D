using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class TooltipShowOnTouch : MonoBehaviour {

    // Use this for initialization
    void Start () {
        setRendererEnabled(false);
        GetComponentInParent<VRTK_InteractableObject>().InteractableObjectTouched += new InteractableObjectEventHandler(DoTouched);
        GetComponentInParent<VRTK_InteractableObject>().InteractableObjectUntouched += new InteractableObjectEventHandler(DoUntouched);
    }

    private void setRendererEnabled(bool enabled)
    {
        gameObject.SetActiveRecursively(enabled);
    }

    private void DoTouched(object sender, InteractableObjectEventArgs e)
    {
        Debug.Log("Tooltip touched");
        setRendererEnabled(true);
    }

    private void DoUntouched(object sender, InteractableObjectEventArgs e)
    {
        Debug.Log("Tooltip untouched");
        setRendererEnabled(false);
    }

    // Update is called once per frame
    void Update () {
        
    }
}
