using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LineController : NetworkBehaviour {

    [SyncVar]
    public Vector3 startPosition;

    [SyncVar]
    public Vector3 endPosition;

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        var line = gameObject.GetComponent<LineRenderer>();
        line.SetPosition(0, startPosition);
        line.SetPosition(1, endPosition);
    }
}
