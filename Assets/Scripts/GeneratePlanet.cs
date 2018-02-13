using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePlanet : MonoBehaviour {

    public Material material;
    public int resolution; // This is *roughly* resolution in pixels, though it can vary generator to generator

    // Use this for initialization
    public virtual void Generate () {
        // Do... nothing!
    }
}
