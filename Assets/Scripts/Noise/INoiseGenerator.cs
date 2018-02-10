using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INoiseGenerator : MonoBehaviour {
    public virtual float ValueAtPoint(Vector3 p)
    {
        return 0.0f;
    }
}
